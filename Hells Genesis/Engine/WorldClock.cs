using System.Diagnostics;
using System.Threading;

namespace HG.Engine
{
    /// <summary>
    /// The world clock. Moves all objects forward in time, renders all objects and keeps the frame-counter in check.
    /// </summary>
    internal class WorldClock
    {
        private readonly Core _core;
        private bool _shutdown = false;
        private bool _pause = false;
        private readonly Thread _graphicsThread;

        public WorldClock(Core core)
        {
            _core = core;
            _graphicsThread = new Thread(GraphicsThreadProc);
        }

        #region Start / Stop / Pause.

        public void Start()
        {
            _shutdown = false;
            _graphicsThread.Start();
        }

        public void Stop()
        {
            _shutdown = true;
            _graphicsThread.Join();
        }

        public bool IsPaused() => _pause;

        public void TogglePause()
        {
            _pause = !_pause;
        }

        public void Pause()
        {
            _pause = true;
        }

        public void Resume()
        {
            _pause = false;
        }

        #endregion

        private void GraphicsThreadProc()
        {
            #region Add initial stars.

            for (int i = 0; i < Settings.InitialStarCount; i++)
            {
                _core.Actors.Stars.Create();
            }

            #endregion

            var timer = new Stopwatch();
            var targetFrameDuration = 1000000 / Settings.FrameLimiter; //1000000 / n-frames/second.

            while (_shutdown == false)
            {
                timer.Restart();

                _core.Display.GameLoopCounter.Calculate();

                lock (_core.Menus._controller)
                    lock (_core.Player.Actor)
                        lock (_core.Actors.Collection)
                        {
                            if (_pause == false)
                            {
                                ExecuteWorldClockTick();
                            }
                            _core.Render();
                            timer.Stop();
                        }

                var clockTime = (((double)timer.ElapsedTicks) / Stopwatch.Frequency) * 1000000;
                var deltaClockTime = targetFrameDuration - clockTime;
                timer.Restart();

                while (((double)timer.ElapsedTicks) / Stopwatch.Frequency * 1000000 < deltaClockTime)
                {
                    Thread.Yield();
                }

                if (_pause)
                {
                    Thread.Yield();
                }
            }
        }

        void ExecuteWorldClockTick()
        {
            _core.Menus.ExecuteWorldClockTick();
            _core.Situations.ExecuteWorldClockTick();
            _core.Events.ExecuteWorldClockTick();

            var displacementVector = _core.Player.ExecuteWorldClockTick();

            _core.Actors.Enemies.ExecuteWorldClockTick(displacementVector);
            _core.Actors.Particles.ExecuteWorldClockTick(displacementVector);
            _core.Actors.RadarPositions.ExecuteWorldClockTick();
            _core.Actors.Bullets.ExecuteWorldClockTick(displacementVector);
            _core.Actors.Stars.ExecuteWorldClockTick(displacementVector);
            _core.Actors.Animations.ExecuteWorldClockTick(displacementVector);
            _core.Actors.TextBlocks.ExecuteWorldClockTick(displacementVector);
            _core.Actors.Powerups.ExecuteWorldClockTick(displacementVector);
            _core.Actors.Debugs.ExecuteWorldClockTick(displacementVector);

            _core.Actors.CleanupDeletedObjects();

            if (_core.Player.Actor.Visable == false)
            {
                _core.Player.Actor.ShipEngineIdleSound.Stop();
                _core.Player.Actor.ShipEngineRoarSound.Stop();
            }

            string situation = "<peaceful>";

            if (_core.Situations.CurrentSituation != null)
            {
                situation = $"{_core.Situations.CurrentSituation.Name} (Wave {_core.Situations.CurrentSituation.CurrentWave} of {_core.Situations.CurrentSituation.TotalWaves})";
            }

            double boostRebuildPercent = (_core.Player.Actor.Velocity.AvailableBoost / Settings.PlayerBoostRebuildMin) * 100.0;

            _core.Actors.PlayerStatsText.Text =
                  $" Situation: {situation}\r\n"
                + $"      Hull: {_core.Player.Actor.HullHealth:n0} (Shields: {_core.Player.Actor.ShieldHealth:n0}) | Bounty: ${_core.Player.Actor.Bounty}\r\n"
                + $"      Warp: {((_core.Player.Actor.Velocity.AvailableBoost / Settings.MaxPlayerBoost) * 100.0):n1}%"
                    + (_core.Player.Actor.Velocity.BoostRebuilding ? $" (RECHARGING: {boostRebuildPercent:n1}%)" : string.Empty) + "\r\n"
                + $"Pri-Weapon: {_core.Player.Actor.PrimaryWeapon?.Name} x{_core.Player.Actor.PrimaryWeapon?.RoundQuantity:n0}\r\n"
                + $"Sec-Weapon: {_core.Player.Actor.SelectedSecondaryWeapon?.Name} x{_core.Player.Actor.SelectedSecondaryWeapon?.RoundQuantity:n0}\r\n";

            if (_core.ShowDebug)
            {
                _core.Actors.DebugText.Text =
                      $"       Frame Rate: Avg: {_core.Display.GameLoopCounter.AverageFrameRate.ToString("0.0")}, "
                                        + $"Min: {_core.Display.GameLoopCounter.FrameRateMin.ToString("0.0")}, "
                                        + $"Max: {_core.Display.GameLoopCounter.FrameRateMax.ToString("0.0")}\r\n"
                    + $"Player Display XY: {_core.Player.Actor.X:#0.00}x, {_core.Player.Actor.Y:#0.00}y\r\n"
                    + $"     Player Angle: {_core.Player.Actor.Velocity.Angle.X:#0.00}x, {_core.Player.Actor.Velocity.Angle.Y:#0.00}y, "
                                        + $"{_core.Player.Actor.Velocity.Angle.Degrees:#0.00}deg, "
                                        + $" {_core.Player.Actor.Velocity.Angle.Radians:#0.00}rad, "
                                        + $" {_core.Player.Actor.Velocity.Angle.RadiansUnadjusted:#0.00}rad unadjusted\r\n"
                    + $"Player Virtual XY: {_core.Player.Actor.X + _core.Display.BackgroundOffset.X:#0.00}x,"
                                        + $" {_core.Player.Actor.Y + _core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                    + $"        BG Offset: {_core.Display.BackgroundOffset.X:#0.00}x, {_core.Display.BackgroundOffset.Y:#0.00}y\r\n"
                    + $"  Delta BG Offset: {displacementVector.X:#0.00}x, {displacementVector.Y:#0.00}y\r\n"
                    + $"           Thrust: {(_core.Player.Actor.Velocity.ThrottlePercentage * 100):#0.00}\r\n"
                    + $"            Boost: {(_core.Player.Actor.Velocity.BoostPercentage * 100):#0.00}\r\n"
                    + $"         Quadrant: {_core.Display.CurrentQuadrant.Key.X}:{_core.Display.CurrentQuadrant.Key.Y}\r\n"
                    + $"           Bounty: {_core.Player.Actor.Bounty.ToString("#,0")}";
            }
            else
            {
                if (_core.Actors.DebugText.Text != string.Empty)
                {
                    _core.Actors.DebugText.Text = string.Empty;
                }
            }
        }
    }
}
