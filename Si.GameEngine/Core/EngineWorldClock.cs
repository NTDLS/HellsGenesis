using Si.GameEngine.Sprites;
using Si.Shared.Types.Geometry;
using System.Diagnostics;
using System.Threading;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Core
{
    /// <summary>
    /// The world clock. Moves all objects forward in time, renders all objects and keeps the frame-counter in check.
    /// </summary>
    internal class EngineWorldClock
    {
        private readonly Engine _gameEngine;
        private bool _shutdown = false;
        private bool _isPaused = false;
        private readonly Thread _graphicsThread;

        public EngineWorldClock(Engine gameEngine)
        {
            _gameEngine = gameEngine;
            _graphicsThread = new Thread(GraphicsThreadProc);
        }

        #region Start / Stop / Pause.

        public void Start()
        {
            _shutdown = false;
            _graphicsThread.Start();
        }

        public void Shutdown()
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

            var timer = new Stopwatch();

            while (_shutdown == false)
            {
                var targetFrameDuration = 1000000 / _gameEngine.Settings.FramePerSecondLimit; //1000000 / n-frames/second.
                timer.Restart();

                _gameEngine.Display.GameLoopCounter.Calculate();

                _gameEngine.Menus.Use(m =>
                {
                    _gameEngine.Sprites.Use(o =>
                    {
                        if (_isPaused == false)
                        {
                            BeforeExecuteWorldClockTick();
                            var displacementVector = ExecuteWorldClockTick();
                            AfterExecuteWorldClockTick(displacementVector);
                        }

                        _gameEngine.Debug.ProcessCommand();

                        _gameEngine.Render();
                        timer.Stop();
                    });
                });

                var clockTime = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000000;
                var deltaClockTime = targetFrameDuration - clockTime;
                timer.Restart();

                while ((double)timer.ElapsedTicks / Stopwatch.Frequency * 1000000 < deltaClockTime)
                {
                    Thread.Yield();
                }

                if (_isPaused)
                {
                    Thread.Yield();
                }
            }
        }

        private SiPoint ExecuteWorldClockTick()
        {
            _gameEngine.Menus.ExecuteWorldClockTick();
            _gameEngine.Situations.ExecuteWorldClockTick();
            _gameEngine.Events.ExecuteWorldClockTick();

            _gameEngine.Input.Snapshot();

            var displacementVector = _gameEngine.Player.ExecuteWorldClockTick();

            _gameEngine.Sprites.Enemies.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.Particles.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.Munitions.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.Stars.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.Animations.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.TextBlocks.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.Powerups.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.Debugs.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.PlayerDrones.ExecuteWorldClockTick(displacementVector);
            _gameEngine.Sprites.EnemyDrones.ExecuteWorldClockTick(displacementVector);

            _gameEngine.Sprites.RadarPositions.ExecuteWorldClockTick();

            _gameEngine.Sprites.CleanupDeletedObjects();

            _gameEngine.Multiplay.FlushSpriteVectorsToServer();

            return displacementVector;
        }

        private void BeforeExecuteWorldClockTick()
        {
        }

        private void AfterExecuteWorldClockTick(SiPoint displacementVector)
        {
            if (_gameEngine.Player.Sprite.Visable == false)
            {
                _gameEngine.Player.Sprite.ShipEngineIdleSound.Stop();
                _gameEngine.Player.Sprite.ShipEngineRoarSound.Stop();
            }

            if (_gameEngine.Multiplay.State.PlayMode != SiPlayMode.MutiPlayerClient
                && _gameEngine.Situations?.CurrentSituation?.State == SiSituationState.Started)
            {
                //situation = $"{_gameEngine.Situations.CurrentSituation.Name} (Wave {_gameEngine.Situations.CurrentSituation.CurrentWave} of {_gameEngine.Situations.CurrentSituation.TotalWaves})";
                string situation = $"{_gameEngine.Situations.CurrentSituation.Name}";

                double boostRebuildPercent = _gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.PlayerBoostRebuildFloor * 100.0;

                _gameEngine.Sprites.PlayerStatsText.Text =
                      $" Situation: {situation}\r\n"
                    + $"      Hull: {_gameEngine.Player.Sprite.HullHealth:n0} (Shields: {_gameEngine.Player.Sprite.ShieldHealth:n0}) | Bounty: ${_gameEngine.Player.Sprite.Bounty}\r\n"
                    + $"      Warp: {_gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.MaxPlayerBoostAmount * 100.0:n1}%"
                        + (_gameEngine.Player.Sprite.Velocity.BoostRebuilding ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                    + $"Pri-Weapon: {_gameEngine.Player.Sprite.PrimaryWeapon?.Name} x{_gameEngine.Player.Sprite.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"Sec-Weapon: {_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.Name} x{_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n";
            }
            else if (_gameEngine.Multiplay.State.PlayMode == SiPlayMode.MutiPlayerClient)
            {
                double boostRebuildPercent = _gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.PlayerBoostRebuildFloor * 100.0;

                _gameEngine.Sprites.PlayerStatsText.Text =
                     $"      Hull: {_gameEngine.Player.Sprite.HullHealth:n0} (Shields: {_gameEngine.Player.Sprite.ShieldHealth:n0}) | Bounty: ${_gameEngine.Player.Sprite.Bounty}\r\n"
                    + $"      Warp: {_gameEngine.Player.Sprite.Velocity.AvailableBoost / _gameEngine.Settings.MaxPlayerBoostAmount * 100.0:n1}%"
                        + (_gameEngine.Player.Sprite.Velocity.BoostRebuilding ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                    + $"Pri-Weapon: {_gameEngine.Player.Sprite.PrimaryWeapon?.Name} x{_gameEngine.Player.Sprite.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"Sec-Weapon: {_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.Name} x{_gameEngine.Player.Sprite.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n";
            }

            //_gameEngine.Sprites.DebugText.Text = "Anything we need to know about?";
        }
    }
}
