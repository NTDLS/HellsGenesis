using System.Diagnostics;
using System.Threading;

namespace HG.Engine
{
    internal class GameLoop
    {
        private readonly Core _core;
        private bool _shutdown = false;
        private bool _pause = false;
        private readonly Thread _graphicsThread;

        public GameLoop(Core core)
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
        }

        public bool IsPaused()
        {
            return _pause;
        }

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

            for (int i = 0; i < 60; i++)
            {
                _core.Actors.Stars.Create();
            }

            #endregion

            var timer = new Stopwatch();
            var targetFrameDuration = 1000000 / _core.Settings.FrameLimiter; //1000000 / n-frames/second.

            while (_shutdown == false)
            {
                timer.Restart();

                _core.Display.GameLoopCounter.Calculate();

                if (_pause == false)
                {
                    Monitor.Enter(_core.DrawingSemaphore);

                    lock (_core.Menus.Collection)
                        lock (_core.Player.Actor)
                            lock (_core.Actors.Collection)
                            {
                                ExecuteWorldClockTick();
                                timer.Stop();

                            }

                    Monitor.Exit(_core.DrawingSemaphore);
                }

                if (_core.Menus.Collection.Count > 0)
                {
                    Thread.Sleep(20);
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
                    _core.Display.DrawingSurface.Invalidate();
                    Thread.Sleep(5);
                }
            }
        }

        void ExecuteWorldClockTick()
        {
            _core.Menus.ExecuteWorldClockTick();
            _core.Situations.ExecuteWorldClockTick();

            var appliedOffset = _core.Player.ExecuteWorldClockTick();

            _core.Events.ExecuteWorldClockTick();
            _core.Actors.Enemies.ExecuteWorldClockTick(appliedOffset);
            _core.Actors.RadarPositions.ExecuteWorldClockTick(appliedOffset);
            _core.Actors.Bullets.ExecuteWorldClockTick(appliedOffset);
            _core.Actors.Stars.ExecuteWorldClockTick(appliedOffset);
            _core.Actors.Animations.ExecuteWorldClockTick(appliedOffset);
            _core.Actors.TextBlocks.ExecuteWorldClockTick(appliedOffset);
            _core.Actors.Powerups.ExecuteWorldClockTick(appliedOffset);
            _core.Actors.Debugs.ExecuteWorldClockTick(appliedOffset);

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

            _core.Actors.PlayerStatsText.Text =
                  $" Situation: {situation}\r\n"
                + $"      Hull: {_core.Player.Actor.HitPoints} (Shields: {_core.Player.Actor.ShieldPoints})\r\n"
                + $"     Boost: {_core.Player.Actor.Velocity.AvailableBoost.ToString("#,0")}\r\n"
                + $"Pri-Weapon: {_core.Player.Actor.SelectedPrimaryWeapon?.Name} x{_core.Player.Actor.SelectedPrimaryWeapon?.RoundQuantity}\r\n"
                + $"Sec-Weapon: {_core.Player.Actor.SelectedSecondaryWeapon?.Name} x{_core.Player.Actor.SelectedSecondaryWeapon?.RoundQuantity}\r\n";

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
                    + $"  Delta BG Offset: {appliedOffset.X:#0.00}x, {appliedOffset.Y:#0.00}y\r\n"
                    + $"           Thrust: {(_core.Player.Actor.Velocity.ThrottlePercentage * 100):#0.00}\r\n"
                    + $"            Boost: {(_core.Player.Actor.Velocity.BoostPercentage * 100):#0.00}\r\n"
                    + $"         Quadrant: {_core.Display.CurrentQuadrant.Key.X}:{_core.Display.CurrentQuadrant.Key.Y}\r\n"
                    + $"            Score: {_core.Player.Actor.Score.ToString("#,0")}";
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
