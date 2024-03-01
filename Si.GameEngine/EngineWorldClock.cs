using Si.GameEngine.Core.Types;
using Si.GameEngine.Sprites;
using Si.Library.Mathematics.Geometry;
using Si.Rendering;
using System;
using System.Diagnostics;
using System.Threading;
using static Si.Library.SiConstants;

namespace Si.GameEngine
{
    /// <summary>
    /// The world clock. Moves all objects forward in time, renders all objects and keeps the frame-counter in check.
    /// </summary>
    internal class EngineWorldClock : IDisposable
    {
        private readonly GameEngineCore _gameEngine;
        private bool _shutdown = false;
        private bool _isPaused = false;
        private readonly Thread _graphicsThread;

        public EngineWorldClock(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;
            _graphicsThread = new Thread(GraphicsThreadProc);
        }

        #region Start / Stop / Pause.

        public void Start()
        {
            _shutdown = false;
            _graphicsThread.Start();

            _gameEngine.Events.Add(10, UpdateStatusText, SiDefermentEvent.SiCallbackEventMode.Recurring);
        }

        public void Dispose()
        {
            _shutdown = true;
            _graphicsThread.Join();
        }

        public bool IsPaused() => _isPaused;

        public void TogglePause()
        {
            if (_gameEngine.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer)
            {
                return; //Cant pause multiplayer games.
            }

            _isPaused = !_isPaused;

            var textBlock = _gameEngine.Sprites.GetSpriteByTag<SpriteTextBlock>("PausedText");
            if (textBlock == null)
            {
                textBlock = _gameEngine.Sprites.TextBlocks.Create(_gameEngine.Rendering.TextFormats.LargeBlocker,
                    _gameEngine.Rendering.Materials.Brushes.Red, new SiPoint(100, 100), true, "PausedText", "Paused");

                textBlock.X = _gameEngine.Display.NatrualScreenSize.Width / 2 - textBlock.Size.Width / 2;
                textBlock.Y = _gameEngine.Display.NatrualScreenSize.Height / 2 - textBlock.Size.Height / 2;
            }

            textBlock.Visable = _isPaused;
        }

        public void Pause()
        {
            if (_gameEngine.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer)
            {
                return; //Cant pause multiplayer games.
            }

            _isPaused = true;
        }

        public void Resume()
        {
            if (_gameEngine.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer)
            {
                return; //Cant pause multiplayer games.
            }
            _isPaused = false;
        }

        #endregion

        private void GraphicsThreadProc()
        {
            #region Add initial stars.

            for (int i = 0; i < _gameEngine.Settings.InitialFrameStarCount; i++)
            {
                _gameEngine.Sprites.Stars.Create();
            }

            #endregion

            var frameRateTimer = new Stopwatch();
            var worldTickTimer = new Stopwatch();
            var epochTimer = new Stopwatch();

            Thread.Sleep((int)_gameEngine.Settings.WorldTicksPerSecond); //Make sure the first epoch isn't instantaneous.

            frameRateTimer.Start();
            worldTickTimer.Start();
            epochTimer.Start();

            var framePerSecondLimit = _gameEngine.Settings.TargetFrameRate;

            if (_gameEngine.Settings.VerticalSync)
            {
                framePerSecondLimit = SiRenderingUtility.GetScreenRefreshRate(_gameEngine.Display.Screen, _gameEngine.Settings.GraphicsAdapterId);
            }

            var frameRateDelayMicroseconds = 1000000 / framePerSecondLimit;
            var targetWorldTickDurationMicroseconds = 1000000 / _gameEngine.Settings.WorldTicksPerSecond;
            var millisecondPerEpoch = 1000 / _gameEngine.Settings.WorldTicksPerSecond;

            int _frameRateAdjustCount = 0;
            int _frameRateAdjustCadence = 100;

            while (_shutdown == false)
            {
                worldTickTimer.Restart();

                var elapsedEpochMilliseconds = (double)epochTimer.ElapsedTicks / Stopwatch.Frequency * 1000.0;
                epochTimer.Restart();

                var epoch = (float)(elapsedEpochMilliseconds / millisecondPerEpoch);

                _gameEngine.Sprites.Use(o =>
                {
                    if (!_isPaused)
                    {
                        ExecuteWorldClockTick(epoch);
                    }

                    _gameEngine.Debug.ProcessCommand();

                    //If it is time to render, then render the frame!.
                    if (frameRateTimer.ElapsedTicks * 1000000.0 / Stopwatch.Frequency > frameRateDelayMicroseconds)
                    {
                        _gameEngine.RenderEverything();
                        frameRateTimer.Restart();
                        _gameEngine.Display.FrameCounter.Calculate();

                        #region Framerate fine-tuning.
                        if (_gameEngine.Settings.FineTuneFramerate)
                        {
                            //From time-to-time we want o check the average framerate and make sure its sane,
                            if (_frameRateAdjustCount > _frameRateAdjustCadence)
                            {
                                _frameRateAdjustCount = 0;
                                if (_gameEngine.Display.FrameCounter.AverageFrameRate < framePerSecondLimit && frameRateDelayMicroseconds > 1000)
                                {
                                    //The framerate is too low, reduce the delay.
                                    frameRateDelayMicroseconds -= 1000;
                                }
                                else if (_gameEngine.Display.FrameCounter.AverageFrameRate > framePerSecondLimit * 1.20)
                                {
                                    //the framerate is too high increase the delay.
                                    frameRateDelayMicroseconds += 25;
                                }
                                //System.Diagnostics.Debug.Print($"{frameRateDelayMicroseconds} -> {framePerSecondLimit} -> {_gameEngine.Display.FrameCounter.AverageFrameRate:n4}");
                            }
                            _frameRateAdjustCount++;
                        }
                        #endregion
                    }
                });

                //Determine how many µs it took to render the scene.
                var actualWorldTickDurationMicroseconds = worldTickTimer.ElapsedTicks * 1000000.0 / Stopwatch.Frequency;

                //Calculate how many µs we need to wait so that we can maintain the configured framerate.
                var varianceWorldTickDurationMicroseconds = targetWorldTickDurationMicroseconds - actualWorldTickDurationMicroseconds;

                worldTickTimer.Restart(); //Use the same timer to wait on the delta µs to expire.

                while (worldTickTimer.ElapsedTicks * 1000000.0 / Stopwatch.Frequency < varianceWorldTickDurationMicroseconds)
                {
                    Thread.Yield();
                }

                if (_isPaused)
                {
                    Thread.Yield();
                }
            }
        }

        private SiPoint ExecuteWorldClockTick(float epoch)
        {
            _gameEngine.Menus.ExecuteWorldClockTick();
            _gameEngine.Situations.ExecuteWorldClockTick();
            _gameEngine.Events.ExecuteWorldClockTick();

            _gameEngine.Input.Snapshot();

            var displacementVector = _gameEngine.Player.ExecuteWorldClockTick(epoch);

            _gameEngine.Sprites.Enemies.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.Particles.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.GenericSprites.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.Munitions.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.Stars.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.Animations.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.TextBlocks.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.Powerups.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.Debugs.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.PlayerDrones.ExecuteWorldClockTick(epoch, displacementVector);
            _gameEngine.Sprites.EnemyDrones.ExecuteWorldClockTick(epoch, displacementVector);

            _gameEngine.Sprites.RadarPositions.ExecuteWorldClockTick();

            _gameEngine.Sprites.CleanupDeletedObjects();

            _gameEngine.Multiplay.FlushSpriteVectorsToServer();

            return displacementVector;
        }

        private void UpdateStatusText(SiDefermentEvent sender, object refObj)
        {
            if (_gameEngine.Multiplay.State.PlayMode != SiPlayMode.MutiPlayerClient
                && _gameEngine.Situations?.CurrentSituation?.State == SiSituationState.Started)
            {
                //situation = $"{_gameEngine.Situations.CurrentSituation.Name} (Wave {_gameEngine.Situations.CurrentSituation.CurrentWave} of {_gameEngine.Situations.CurrentSituation.TotalWaves})";
                string situation = $"{_gameEngine.Situations.CurrentSituation.Name}";

                float boostRebuildPercent = _gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.PlayerBoostRebuildFloor * 100.0f;

                _gameEngine.Sprites.PlayerStatsText.Text =
                      $" Situation: {situation}\r\n"
                    + $"      Hull: {_gameEngine.Player.Sprite.HullHealth:n0} (Shields: {_gameEngine.Player.Sprite.ShieldHealth:n0}) | Bounty: ${_gameEngine.Player.Sprite.Bounty}\r\n"
                    + $"     Surge: {_gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.MaxPlayerBoostAmount * 100.0:n1}%"
                        + (_gameEngine.Player.Sprite.Velocity.BoostRebuilding ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                    + $"Pri-Weapon: {_gameEngine.Player.Sprite.PrimaryWeapon?.Name} x{_gameEngine.Player.Sprite.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"Sec-Weapon: {_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.Name} x{_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n";
            }
            else if (_gameEngine.Multiplay.State.PlayMode == SiPlayMode.MutiPlayerClient)
            {
                float boostRebuildPercent = _gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.PlayerBoostRebuildFloor * 100.0f;

                _gameEngine.Sprites.PlayerStatsText.Text =
                      $"      Hull: {_gameEngine.Player.Sprite.HullHealth:n0} (Shields: {_gameEngine.Player.Sprite.ShieldHealth:n0}) | Bounty: ${_gameEngine.Player.Sprite.Bounty}\r\n"
                    + $"     Surge: {_gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.MaxPlayerBoostAmount * 100.0:n1}%"
                        + (_gameEngine.Player.Sprite.Velocity.BoostRebuilding ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                    + $"Pri-Weapon: {_gameEngine.Player.Sprite.PrimaryWeapon?.Name} x{_gameEngine.Player.Sprite.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"Sec-Weapon: {_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.Name} x{_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n";
            }

            //_gameEngine.Sprites.DebugText.Text = "Anything we need to know about?";
        }
    }
}
