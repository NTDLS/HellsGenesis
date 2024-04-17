using NTDLS.DelegateThreadPooling;
using Si.Engine.Core.Types;
using Si.Engine.Manager;
using Si.Engine.TickController._Superclass;
using Si.GameEngine.TickController.VectoredTickController.Uncollidable;
using Si.Library;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
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

        private readonly DelegateThreadPool _worldClockThreadPool;

        public EngineWorldClock(EngineCore engine)
        {
            _worldClockThreadPool = new(engine.Settings.WorldClockThreads);

            engine.OnShutdown += (sender) =>
            {
                _worldClockThreadPool.Stop();
            };

            _engine = engine;
            _graphicsThread = new Thread(GraphicsThreadProc);
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

            _engine.Sprites.TextBlocks.PausedText.X = _engine.Display.NatrualScreenSize.Width / 2 - _engine.Sprites.TextBlocks.PausedText.Size.Width / 2;
            _engine.Sprites.TextBlocks.PausedText.Y = _engine.Display.NatrualScreenSize.Height / 2 - _engine.Sprites.TextBlocks.PausedText.Size.Height / 2;
            _engine.Sprites.TextBlocks.PausedText.Visable = _isPaused;
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

                var elapsedFrameTime = _engine.Display.FrameCounter.ElapsedMicroseconds;

                // Enforce the framerate by figuring out how long it took to render the frame,
                //  then spin for the difference between how long we wanted it to take.
                while (_engine.Display.FrameCounter.ElapsedMicroseconds - elapsedFrameTime < targetTimePerFrameMicroseconds - elapsedFrameTime)
                {
                    if (_engine.Settings.YeildRemainingFrameTime) Thread.Yield();
                }

                if (_isPaused) Thread.Yield();

                elapsedEpochMilliseconds = _engine.Display.FrameCounter.ElapsedMilliseconds;

                _engine.Display.FrameCounter.Calculate();
            }
        }

        private SiVector ExecuteWorldClockTick(float epoch)
        {
            //This is where we execute the world clock for each type of object.
            //Note that this function does employ threads but I DO NOT beleive it is necessary for performance.
            //
            //The idea is that sprites are created in Events.ExecuteWorldClockTick(), input is snapshotted,
            //  the player is moved (so we have a displacment vector) and then we should be free to do all
            //  other operations in paralell with the exception of deleting sprites that are queued for deletion -
            //  which is why we do that after the threads have completed.

            _engine.Collisions.Reset(epoch);

            _engine.Menus.ExecuteWorldClockTick();
            _engine.Situations.ExecuteWorldClockTick();
            _engine.Events.ExecuteWorldClockTick();

            _engine.Input.Snapshot();

            var displacementVector = _engine.Player.ExecuteWorldClockTick(epoch);

            //Create a collection of threads so we can wait on the ones that we start.
            var threadPoolTracker = _worldClockThreadPool.CreateQueueStateTracker();

            threadPoolTracker.Enqueue(() => _engine.Sprites.Enemies.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.Particles.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.GenericSprites.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.Attachments.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.Munitions.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.Stars.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.SkyBoxes.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.Animations.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.TextBlocks.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.Powerups.ExecuteWorldClockTick(epoch, displacementVector));
            threadPoolTracker.Enqueue(() => _engine.Sprites.Debugs.ExecuteWorldClockTick(epoch, displacementVector));

            //Wait on all enqueued threads to complete.
            if (!SiUtility.TryAndIgnore(threadPoolTracker.WaitForCompletion))
            {
                return displacementVector; //This is kind of an exception, it likely means that the engine is shutting down - so just return.
            }

            _engine.Sprites.RadarPositions.ExecuteWorldClockTick();

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
