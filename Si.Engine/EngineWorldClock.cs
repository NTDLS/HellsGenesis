using NTDLS.DelegateThreadPooling;
using Si.Engine.Core.Types;
using Si.Engine.Manager;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using static Si.Library.SiConstants;

namespace Si.Engine
{
    /// <summary>
    /// The world clock. Moves all objects forward in time, renders all objects and keeps the frame-counter in check.
    /// </summary>
    internal class EngineWorldClock : IDisposable
    {
        private readonly EngineCore _engine;
        private bool _shutdown = false;
        private bool _isPaused = false;
        private readonly Thread _graphicsThread;
        private TrackableQueue<TickControllerMethod> _threadPoolTracker = null;

        private readonly DelegateThreadPool _worldClockThreadPool;

        private struct TickControllerMethod
        {
            public object Controller;
            public MethodInfo Method;

            public TickControllerMethod(object controller, MethodInfo method)
            {
                Controller = controller;
                Method = method;
            }
        }

        private readonly List<TickControllerMethod> _vectoredTickControllers = new();
        private readonly List<TickControllerMethod> _unvectoredTickControllers = new();

        public EngineWorldClock(EngineCore engine)
        {
            _engine = engine;
            _worldClockThreadPool = new(engine.Settings.WorldClockThreads);

            engine.OnShutdown += (sender) =>
            {
                _worldClockThreadPool.Stop();
            };

            _graphicsThread = new Thread(GraphicsThreadProc);

            if (_engine.Settings.MultithreadedWorldClock)
            {
                //Create a collection of threads so we can wait on the ones that we start.
                _threadPoolTracker ??= _worldClockThreadPool.CreateChildQueue<TickControllerMethod>();
            }

            #region Cache vectored and unvectored tick controller methods.

            var properties = typeof(SpriteManager).GetProperties();

            foreach (var property in properties)
            {
                if (SiReflection.IsAssignableToGenericType(property.PropertyType, typeof(VectoredTickControllerBase<>))
                    || SiReflection.IsAssignableToGenericType(property.PropertyType, typeof(VectoredCollidableTickControllerBase<>)))
                {
                    var method = property.PropertyType.GetMethod("ExecuteWorldClockTick")
                        ?? throw new Exception("VectoredTickController must contain ExecuteWorldClockTick");

                    var instance = property.GetValue(_engine.Sprites)
                        ?? throw new Exception($"Sprite manager must contain property [{property.Name}] and it must not bu NULL.");

                    _vectoredTickControllers.Add(new TickControllerMethod(instance, method));

                }
                else if (SiReflection.IsAssignableToGenericType(property.PropertyType, typeof(UnvectoredTickControllerBase<>)))
                {
                    var method = property.PropertyType.GetMethod("ExecuteWorldClockTick")
                        ?? throw new Exception("VectoredTickController must contain ExecuteWorldClockTick");

                    var instance = property.GetValue(_engine.Sprites)
                        ?? throw new Exception($"Sprite manager must contain property [{property.Name}] and it must not bu NULL.");

                    _unvectoredTickControllers.Add(new TickControllerMethod(instance, method));
                }
            }

            #endregion
        }

        #region Start / Stop / Pause.

        public void Start()
        {
            _shutdown = false;
            _graphicsThread.Start();

            _engine.Events.Add(10, UpdateStatusText, SiDefermentEvent.SiDefermentEventMode.Recurring);
        }

        public void Dispose()
        {
            _shutdown = true;
            _graphicsThread.Join();
        }

        public bool IsPaused() => _isPaused;

        public void TogglePause()
        {
            _isPaused = !_isPaused;

            _engine.Sprites.TextBlocks.PausedText.X = _engine.Display.NaturalScreenSize.Width / 2 - _engine.Sprites.TextBlocks.PausedText.Size.Width / 2;
            _engine.Sprites.TextBlocks.PausedText.Y = _engine.Display.NaturalScreenSize.Height / 2 - _engine.Sprites.TextBlocks.PausedText.Size.Height / 2;
            _engine.Sprites.TextBlocks.PausedText.Visible = _isPaused;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        #endregion

        private void GraphicsThreadProc()
        {
            #region Add initial stars.

            for (int i = 0; i < _engine.Settings.InitialFrameStarCount; i++)
            {
                _engine.Sprites.Stars.Add();
            }

            #endregion

            var framePerSecondLimit = _engine.Settings.VerticalSync ?
                SiRenderingUtility.GetScreenRefreshRate(_engine.Display.Screen, _engine.Settings.GraphicsAdapterId)
                : _engine.Settings.TargetFrameRate;

            float targetTimePerFrameMicroseconds = 1000000.0f / framePerSecondLimit;
            float elapsedEpochMilliseconds = 100;

            while (_shutdown == false)
            {
                var epoch = (float)(elapsedEpochMilliseconds / _engine.Settings.MillisecondPerEpoch);

                if (!_isPaused) ExecuteWorldClockTick(epoch);

                _engine.Debug.ProcessCommand();
                _engine.RenderEverything();

                if (_engine.Settings.VerticalSync == false)
                {
                    var elapsedFrameTime = _engine.Display.FrameCounter.ElapsedMicroseconds;

                    // Enforce the framerate by figuring out how long it took to render the frame,
                    //  then spin for the difference between how long we wanted it to take.
                    while (_engine.Display.FrameCounter.ElapsedMicroseconds - elapsedFrameTime < targetTimePerFrameMicroseconds - elapsedFrameTime)
                    {
                        if (_engine.Settings.YieldRemainingFrameTime) Thread.Yield();
                    }
                }

                if (_isPaused) Thread.Yield();

                elapsedEpochMilliseconds = _engine.Display.FrameCounter.ElapsedMilliseconds;

                _engine.Display.FrameCounter.Calculate();
            }
        }

        private SiVector ExecuteWorldClockTick(float epoch)
        {
            _engine.Settings.MultithreadedWorldClock = false;

            //This is where we execute the world clock for each type of object.
            //Note that this function does employ threads but I DO NOT believe it is necessary for performance.
            //
            //The idea is that sprites are created in Events.ExecuteWorldClockTick(), input is snapshotted,
            //  the player is moved (so we have a displacement vector) and then we should be free to do all
            //  other operations in parallel with the exception of deleting sprites that are queued for deletion -
            //  which is why we do that after the threads have completed.

            _engine.Collisions.Reset(epoch);

            _engine.Menus.ExecuteWorldClockTick();
            _engine.Situations.ExecuteWorldClockTick();
            _engine.Events.ExecuteWorldClockTick();

            _engine.Input.Snapshot();

            var displacementVector = _engine.Player.ExecuteWorldClockTick(epoch);

            //Enqueue each vectored tick controller for a thread.
            var vectoredParameters = new object[] { epoch, displacementVector };
            if (_engine.Settings.MultithreadedWorldClock)
            {
                foreach (var vectored in _vectoredTickControllers)
                {
                    _threadPoolTracker.Enqueue(vectored,
                        (TickControllerMethod p) => p.Method.Invoke(p.Controller, vectoredParameters));
                }

                //Wait on all enqueued threads to complete.
                if (!SiUtility.TryAndIgnore(_threadPoolTracker.WaitForCompletion))
                {
                    return displacementVector; //This is kind of an exception, it likely means that the engine is shutting down - so just return.
                }
            }
            else
            {
                foreach (var vectored in _vectoredTickControllers)
                {
                    vectored.Method.Invoke(vectored.Controller, vectoredParameters);
                }
            }

            //After all vectored tick controllers have executed, run the unvectored tick controllers.
            if (_engine.Settings.MultithreadedWorldClock)
            {
                foreach (var unvectored in _unvectoredTickControllers)
                {
                    _threadPoolTracker.Enqueue(unvectored,
                        (TickControllerMethod p) => p.Method.Invoke(p.Controller, null));
                }

                //Wait on all enqueued threads to complete.
                if (!SiUtility.TryAndIgnore(_threadPoolTracker.WaitForCompletion))
                {
                    return displacementVector; //This is kind of an exception, it likely means that the engine is shutting down - so just return.
                }
            }
            else
            {
                foreach (var vectored in _unvectoredTickControllers)
                {
                    vectored.Method.Invoke(vectored.Controller, null);
                }
            }

            _engine.Sprites.HardDeleteAllQueuedDeletions();

            return displacementVector;
        }

        private void UpdateStatusText(SiDefermentEvent sender, object refObj)
        {
            if (_engine.Situations?.CurrentSituation?.State == SiSituationState.Started)
            {
                //situation = $"{_engine.Situations.CurrentSituation.Name} (Wave {_engine.Situations.CurrentSituation.CurrentWave} of {_engine.Situations.CurrentSituation.TotalWaves})";
                string situation = $"{_engine.Situations.CurrentSituation.Name}";

                var player = _engine.Player.Sprite;

                var boost = player.RenewableResources.Snapshot(player.BoostResourceName);

                float boostRebuildPercent = boost.RawAvailableResource / boost.CooldownFloor * 100.0f;

                _engine.Sprites.TextBlocks.PlayerStatsText.Text =
                      $" Situation: {situation}\r\n"
                    + $"      Hull: {player.HullHealth:n0} (Shields: {player.ShieldHealth:n0}) | Bounty: ${player.Metadata.Bounty}\r\n"
                    + $"     Surge: {boost.RawAvailableResource / _engine.Settings.MaxPlayerBoostAmount * 100.0:n1}%"
                    + (boost.IsCoolingDown ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                    + $"Pri-Weapon: {player.PrimaryWeapon?.Metadata.Name} x{player.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"Sec-Weapon: {player.SelectedSecondaryWeapon?.Metadata.Name} x{player.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"{_engine.Display.FrameCounter.AverageFrameRate:n2}fps";
            }

            //_engine.Sprites.DebugText.Text = "Anything we need to know about?";
        }
    }
}
