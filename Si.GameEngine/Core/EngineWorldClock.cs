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
        private readonly Engine _gameCore;
        private bool _shutdown = false;
        private bool _isPaused = false;
        private readonly Thread _graphicsThread;

        public EngineWorldClock(Engine gameCore)
        {
            _gameCore = gameCore;
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
            if (_gameCore.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer)
            {
                return; //Cant pause multiplayer games.
            }

            _isPaused = !_isPaused;

            var textBlock = _gameCore.Sprites.GetSpriteByTag<SpriteTextBlock>("PausedText");
            if (textBlock == null)
            {
                textBlock = _gameCore.Sprites.TextBlocks.Create(_gameCore.Rendering.TextFormats.LargeBlocker,
                    _gameCore.Rendering.Materials.Brushes.Red, new SiPoint(100, 100), true, "PausedText", "Paused");

                textBlock.X = _gameCore.Display.NatrualScreenSize.Width / 2 - textBlock.Size.Width / 2;
                textBlock.Y = _gameCore.Display.NatrualScreenSize.Height / 2 - textBlock.Size.Height / 2;
            }

            textBlock.Visable = _isPaused;
        }

        public void Pause()
        {
            if (_gameCore.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer)
            {
                return; //Cant pause multiplayer games.
            }

            _isPaused = true;
        }

        public void Resume()
        {
            if (_gameCore.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer)
            {
                return; //Cant pause multiplayer games.
            }
            _isPaused = false;
        }

        #endregion

        private void GraphicsThreadProc()
        {
            #region Add initial stars.

            for (int i = 0; i < _gameCore.Settings.InitialFrameStarCount; i++)
            {
                _gameCore.Sprites.Stars.Create();
            }

            #endregion

            var timer = new Stopwatch();

            while (_shutdown == false)
            {
                var targetFrameDuration = 1000000 / _gameCore.Settings.FramePerSecondLimit; //1000000 / n-frames/second.
                timer.Restart();

                _gameCore.Display.GameLoopCounter.Calculate();

                _gameCore.Menus.Use(m =>
                {
                    _gameCore.Sprites.Use(o =>
                    {
                        if (_isPaused == false)
                        {
                            BeforeExecuteWorldClockTick();
                            var displacementVector = ExecuteWorldClockTick();
                            AfterExecuteWorldClockTick(displacementVector);
                        }

                        _gameCore.Debug.ProcessCommand();

                        _gameCore.Render();
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
            _gameCore.Menus.ExecuteWorldClockTick();
            _gameCore.Situations.ExecuteWorldClockTick();
            _gameCore.Events.ExecuteWorldClockTick();

            _gameCore.Input.Snapshot();

            var displacementVector = _gameCore.Player.ExecuteWorldClockTick();

            _gameCore.Sprites.Enemies.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.Particles.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.Munitions.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.Stars.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.Animations.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.TextBlocks.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.Powerups.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.Debugs.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.PlayerDrones.ExecuteWorldClockTick(displacementVector);
            _gameCore.Sprites.EnemyDrones.ExecuteWorldClockTick(displacementVector);

            _gameCore.Sprites.RadarPositions.ExecuteWorldClockTick();

            _gameCore.Sprites.CleanupDeletedObjects();

            _gameCore.Multiplay.FlushSpriteVectorsToServer();

            return displacementVector;
        }

        private void BeforeExecuteWorldClockTick()
        {
        }

        private void AfterExecuteWorldClockTick(SiPoint displacementVector)
        {
            if (_gameCore.Player.Sprite.Visable == false)
            {
                _gameCore.Player.Sprite.ShipEngineIdleSound.Stop();
                _gameCore.Player.Sprite.ShipEngineRoarSound.Stop();
            }

            if (_gameCore.Multiplay.State.PlayMode != SiPlayMode.MutiPlayerClient
                && _gameCore.Situations?.CurrentSituation?.State == SiSituationState.Started)
            {
                //situation = $"{_gameCore.Situations.CurrentSituation.Name} (Wave {_gameCore.Situations.CurrentSituation.CurrentWave} of {_gameCore.Situations.CurrentSituation.TotalWaves})";
                string situation = $"{_gameCore.Situations.CurrentSituation.Name}";

                double boostRebuildPercent = _gameCore.Player.Sprite.Velocity.AvailableBoost / _gameCore.Settings.PlayerBoostRebuildFloor * 100.0;

                _gameCore.Sprites.PlayerStatsText.Text =
                      $" Situation: {situation}\r\n"
                    + $"      Hull: {_gameCore.Player.Sprite.HullHealth:n0} (Shields: {_gameCore.Player.Sprite.ShieldHealth:n0}) | Bounty: ${_gameCore.Player.Sprite.Bounty}\r\n"
                    + $"      Warp: {_gameCore.Player.Sprite.Velocity.AvailableBoost / _gameCore.Settings.MaxPlayerBoostAmount * 100.0:n1}%"
                        + (_gameCore.Player.Sprite.Velocity.BoostRebuilding ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                    + $"Pri-Weapon: {_gameCore.Player.Sprite.PrimaryWeapon?.Name} x{_gameCore.Player.Sprite.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"Sec-Weapon: {_gameCore.Player.Sprite.SelectedSecondaryWeapon?.Name} x{_gameCore.Player.Sprite.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n";
            }
            else if (_gameCore.Multiplay.State.PlayMode == SiPlayMode.MutiPlayerClient)
            {
                double boostRebuildPercent = _gameCore.Player.Sprite.Velocity.AvailableBoost / _gameCore.Settings.PlayerBoostRebuildFloor * 100.0;

                _gameCore.Sprites.PlayerStatsText.Text =
                     $"      Hull: {_gameCore.Player.Sprite.HullHealth:n0} (Shields: {_gameCore.Player.Sprite.ShieldHealth:n0}) | Bounty: ${_gameCore.Player.Sprite.Bounty}\r\n"
                    + $"      Warp: {_gameCore.Player.Sprite.Velocity.AvailableBoost / _gameCore.Settings.MaxPlayerBoostAmount * 100.0:n1}%"
                        + (_gameCore.Player.Sprite.Velocity.BoostRebuilding ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                    + $"Pri-Weapon: {_gameCore.Player.Sprite.PrimaryWeapon?.Name} x{_gameCore.Player.Sprite.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                    + $"Sec-Weapon: {_gameCore.Player.Sprite.SelectedSecondaryWeapon?.Name} x{_gameCore.Player.Sprite.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n";
            }

            //_gameCore.Sprites.DebugText.Text = "Anything we need to know about?";
        }
    }
}
