using AI2D.GraphicObjects.Enemies;
using AI2D.Types;
using AI2D.Weapons;
using System.Diagnostics;

namespace AI2D.Engine
{
    public class EngineThread
    {
        private Core _core;
        private bool _shutdown = false;
        private bool _pause = false;
        private System.Threading.Thread _graphicsThread;

        public EngineThread(Core core)
        {
            _core = core;

            _graphicsThread = new System.Threading.Thread(GraphicsThreadProc);
        }

        public void Start()
        {
            _shutdown = false;
            _graphicsThread.Start();
        }

        public void Stop()
        {
            _shutdown = true;
        }

        public void Pause()
        {
            _pause = true;
        }

        public void Resume()
        {
            _pause = false;
        }

        private void GraphicsThreadProc()
        {
            for (int i = 0; i < 20; i++)
            {
                _core.Actors.CreateStar();
            }

            Stopwatch timer = new Stopwatch();

            double targetFrameDuration = 1000000 / Constants.Limits.FrameLimiter; //1000000 / n-frames/second.
            bool sleep = true;

            int numberOfTimesLagged = 0;

            while (_shutdown == false)
            {
                _core.Display.GameLoopCounter.Calculate();

                timer.Restart();
                AdvanceFrame();
                if (sleep)
                {
                    System.Threading.Thread.Sleep(1);
                }
                timer.Stop();

                double frameTime = (((double)timer.ElapsedTicks) / Stopwatch.Frequency) * 1000000;
                double deltaframeTime = targetFrameDuration - frameTime;
                timer.Restart();

                if (deltaframeTime < 0)
                {
                    if (++numberOfTimesLagged > 10)
                    {
                        sleep = false; //Unleash the thread!
                    }
                }
                               
                while ((((double)timer.ElapsedTicks) / Stopwatch.Frequency) * 1000000 < deltaframeTime)
                {
                    System.Threading.Thread.Yield();
                }

                timer.Stop();

                while (_pause && _shutdown == false)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        void AdvanceFrame()
        {
            #region Player Frame Advancement.

            PointD appliedOffset = new PointD();

            if (_core.Actors.Player.Visable)
            {
                _core.Actors.Player.IsLockedOn = false;
                _core.Actors.Player.IsLockedOnSoft = false;

                if (_core.Input.IsKeyPressed(PlayerKey.Fire))
                {
                    if (_core.Actors.Player.CurrentWeapon != null && _core.Actors.Player.CurrentWeapon.Fire())
                    {
                        if (_core.Actors.Player.CurrentWeapon?.RoundQuantity == 25)
                        {
                            _core.Actors.Player.AmmoLowSound.Play();
                        }
                        if (_core.Actors.Player.CurrentWeapon?.RoundQuantity == 0)
                        {
                            _core.Actors.Player.AmmoEmptySound.Play();
                            _core.Actors.Player.SelectFirstAvailableUsableWeapon();
                        }
                    }
                }

                //Make player thrust "build up" and fade.
                if (_core.Input.IsKeyPressed(PlayerKey.Forward))
                {
                    if (_core.Actors.Player.Velocity.ThrottlePercentage < 100)
                    {
                        _core.Actors.Player.Velocity.ThrottlePercentage += Constants.PlayerThrustRampUp;
                    }
                }
                else if (_core.Input.IsKeyPressed(PlayerKey.Reverse))
                {
                    if (_core.Actors.Player.Velocity.ThrottlePercentage > -0.5)
                    {
                        _core.Actors.Player.Velocity.ThrottlePercentage -= Constants.PlayerThrustRampUp;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the thrust.
                    if (_core.Actors.Player.Velocity.ThrottlePercentage > 0)
                    {
                        _core.Actors.Player.Velocity.ThrottlePercentage -= Constants.PlayerThrustRampDown;
                        if (_core.Actors.Player.Velocity.ThrottlePercentage < 0)
                        {
                            _core.Actors.Player.Velocity.ThrottlePercentage = 0;
                        }
                    }
                    else if (_core.Actors.Player.Velocity.ThrottlePercentage < 0)
                    {
                        _core.Actors.Player.Velocity.ThrottlePercentage += Constants.PlayerThrustRampDown;
                        if (_core.Actors.Player.Velocity.ThrottlePercentage > 0)
                        {
                            _core.Actors.Player.Velocity.ThrottlePercentage = 0;
                        }
                    }
                }

                if (_core.Actors.Player.Velocity.ThrottlePercentage > 0)
                {
                    double forwardThrust = (_core.Actors.Player.Velocity.MaxSpeed * _core.Actors.Player.Velocity.ThrottlePercentage);

                    //Close to the right wall and travelling in that direction.
                    if (_core.Actors.Player.X > _core.Display.VisibleSize.Width - (_core.Actors.Player.Size.Width + Constants.Limits.InfiniteScrollWall)
                        && _core.Actors.Player.Velocity.Angle.X > 0)
                    {
                        appliedOffset.X = (_core.Actors.Player.Velocity.Angle.X * forwardThrust);
                    }

                    //Close to the bottom wall and travelling in that direction.
                    if (_core.Actors.Player.Y > _core.Display.VisibleSize.Height - (_core.Actors.Player.Size.Height + Constants.Limits.InfiniteScrollWall)
                        && _core.Actors.Player.Velocity.Angle.Y > 0)
                    {
                        appliedOffset.Y = (_core.Actors.Player.Velocity.Angle.Y * forwardThrust);
                    }

                    //Close to the left wall and travelling in that direction.
                    if (_core.Actors.Player.X < Constants.Limits.InfiniteScrollWall && _core.Actors.Player.Velocity.Angle.X < 0)
                    {
                        appliedOffset.X = (_core.Actors.Player.Velocity.Angle.X * forwardThrust);
                    }

                    //Close to the top wall and travelling in that direction.
                    if (_core.Actors.Player.Y < Constants.Limits.InfiniteScrollWall && _core.Actors.Player.Velocity.Angle.Y < 0)
                    {
                        appliedOffset.Y = (_core.Actors.Player.Velocity.Angle.Y * forwardThrust);
                    }

                    _core.Actors.Player.X += (_core.Actors.Player.Velocity.Angle.X * forwardThrust) - appliedOffset.X;
                    _core.Actors.Player.Y += (_core.Actors.Player.Velocity.Angle.Y * forwardThrust) - appliedOffset.Y;

                    _core.Actors.Player.ShipEngineRoarSound.Play();
                }
                else if (_core.Actors.Player.Velocity.ThrottlePercentage < 0)
                {
                    //Do we really need to reverse? This is space!

                    double reverseThrust = -(_core.Actors.Player.Velocity.MaxSpeed * _core.Actors.Player.Velocity.ThrottlePercentage);

                    //Close to the right wall and travelling in that direction.
                    if (_core.Actors.Player.X > _core.Display.VisibleSize.Width - (_core.Actors.Player.Size.Width + Constants.Limits.InfiniteScrollWall)
                        && _core.Actors.Player.Velocity.Angle.X < 0)
                    {
                        appliedOffset.X = -(_core.Actors.Player.Velocity.Angle.X * reverseThrust);
                    }

                    //Close to the bottom wall and travelling in that direction.
                    if (_core.Actors.Player.Y > _core.Display.VisibleSize.Height - (_core.Actors.Player.Size.Height + Constants.Limits.InfiniteScrollWall)
                        && _core.Actors.Player.Velocity.Angle.Y < 0)
                    {
                        appliedOffset.Y = -(_core.Actors.Player.Velocity.Angle.Y * reverseThrust);
                    }

                    //Close to the left wall and travelling in that direction.
                    if (_core.Actors.Player.X < Constants.Limits.InfiniteScrollWall && _core.Actors.Player.Velocity.Angle.X > 0)
                    {
                        appliedOffset.X = -(_core.Actors.Player.Velocity.Angle.X * reverseThrust);
                    }

                    //Close to the top wall and travelling in that direction.
                    if (_core.Actors.Player.Y < Constants.Limits.InfiniteScrollWall && _core.Actors.Player.Velocity.Angle.Y > 0)
                    {
                        appliedOffset.Y = -(_core.Actors.Player.Velocity.Angle.Y * reverseThrust);
                    }

                    _core.Actors.Player.X -= (_core.Actors.Player.Velocity.Angle.X * reverseThrust) + appliedOffset.X;
                    _core.Actors.Player.Y -= (_core.Actors.Player.Velocity.Angle.Y * reverseThrust) + appliedOffset.Y;

                    _core.Actors.Player.ShipEngineRoarSound.Play();
                }
                else
                {
                    _core.Actors.Player.ShipEngineRoarSound.Fade();
                }

                //Scroll the background.
                _core.Display.BackgroundOffset.X += appliedOffset.X;
                _core.Display.BackgroundOffset.Y += appliedOffset.Y;

                //We are going to restrict the rotation speed to a percentage of thrust.
                double rotationSpeed = _core.Actors.Player.Velocity.MaxRotationSpeed * _core.Actors.Player.Velocity.ThrottlePercentage;

                if (_core.Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
                {
                    _core.Actors.Player.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (_core.Input.IsKeyPressed(PlayerKey.RotateClockwise))
                {
                    _core.Actors.Player.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }
            }

            #endregion

            #region Quadrant Math.

            _core.Display.CurrentQuadrant = _core.Display.GetQuadrant(
                _core.Actors.Player.X + _core.Display.BackgroundOffset.X,
                _core.Actors.Player.Y + _core.Display.BackgroundOffset.Y);

            /*
            _core.Actors.DebugText.Text =
                    $"       Frame Rate: Avg: {_core.Display.GameLoopCounter.AverageFrameRate.ToString("0.0")},"
                                    + $"Min: {_core.Display.GameLoopCounter.FrameRateMin.ToString("0.0")},"
                                    + $"Max: {_core.Display.GameLoopCounter.FrameRateMax.ToString("0.0")}\r\n"
                + $"Player Display XY: {_core.Actors.Player.X.ToString("#0.00")}x, {_core.Actors.Player.Y.ToString("#0.00")}y\r\n"
                + $"     Player Angle: {_core.Actors.Player.Velocity.Angle.X.ToString("#0.00")}x, {_core.Actors.Player.Velocity.Angle.Y.ToString("#0.00")}y, "
                                    + $"{_core.Actors.Player.Velocity.Angle.Degrees.ToString("#0.00")}deg, "
                                    + $" {_core.Actors.Player.Velocity.Angle.Radians.ToString("#0.00")}rad, "
                                    + $" {_core.Actors.Player.Velocity.Angle.RadiansUnadjusted.ToString("#0.00")}rad unadjusted\r\n"
                + $"Player Virtual XY: {(_core.Actors.Player.X + _core.Display.BackgroundOffset.X).ToString("#0.00")}x,"
                                    + $" {(_core.Actors.Player.Y + _core.Display.BackgroundOffset.Y).ToString("#0.00")}y\r\n"
                + $"        BG Offset: {_core.Display.BackgroundOffset.X.ToString("#0.00")}x, {_core.Display.BackgroundOffset.Y.ToString("#0.00")}y\r\n"
                + $"  Delta BG Offset: {appliedOffset.X.ToString("#0.00")}x, {appliedOffset.Y.ToString("#0.00")}y\r\n"
                + $"            Thrust: {(_core.Actors.Player.Velocity.ThrottlePercentage * 100).ToString("#0.00")}r\n"
                + $"          Quadrant: {_core.Display.CurrentQuadrant.Key.X}:{_core.Display.CurrentQuadrant.Key.Y}";
            */

            if (_core.Actors.Debugs.Count > 0)
            {
                var angleTo = _core.Actors.Player.AngleTo(_core.Actors.Debugs[0]);
                var deltaAngle = _core.Actors.Player.DeltaAngle(_core.Actors.Debugs[0]);

                var angleToXY = AngleD.DegreesToXY(angleTo);

                _core.Actors.DebugText.Text = $"angleTo: {angleTo.ToString("#,##")} ( {angleToXY.X.ToString("#,##")}x,{angleToXY.Y.ToString("#,##")}y ) "
                    + $"deltaAngle: {deltaAngle.ToString("#,##")}";
            }

            if (_core.Display.CurrentQuadrant.Key.X == -5
                && _core.Display.CurrentQuadrant.Key.Y == -5)
            {
                if (_core.Actors.Enemies.Count < 10)
                {
                    _core.Actors.CreateEnemy<EnemyScinzad>();
                }
            }
            if (_core.Display.CurrentQuadrant.Key.X == 5
                && _core.Display.CurrentQuadrant.Key.Y == 5)
            {
                if (_core.Actors.Enemies.Count < 10)
                {
                    var enemy = _core.Actors.CreateEnemy<EnemyScinzad>();

                    enemy.AddWeapon(new WeaponPhotonTorpedo(_core)
                    {
                        RoundQuantity = 10, //Could make the enemy retreat after running out of ammo?
                        FireDelayMilliseconds = 500,
                    });

                    enemy.AddWeapon(new WeaponVulcanCannon(_core)
                    {
                        FireDelayMilliseconds = 250,
                        //RoundQuantity = 100 //Could make the enemy retreat after running out of ammo?
                    });

                    enemy.SelectWeapon(typeof(WeaponPhotonTorpedo));
                }
            }

            #endregion

            #region Engine Event Callbacks.

            lock (_core.Actors.EngineEvents)
            {
                foreach (var engineEvent in _core.Actors.EngineEvents)
                {
                    engineEvent.CheckForTrigger();
                }
            }

            #endregion

            #region Enemies Frame Advancement.

            lock (_core.Actors.Enemies)
            {
                if (_core.Actors.Player.CurrentWeapon != null)
                {
                    _core.Actors.Player.CurrentWeapon.LockedOnObjects.Clear();
                }

                foreach (var enemy in _core.Actors.Enemies)
                {
                    if (enemy.CurrentWeapon != null)
                    {
                        enemy.CurrentWeapon.LockedOnObjects.Clear();
                    }

                    enemy.IsLockedOn = false;
                    enemy.IsLockedOnSoft = false;

                    if (enemy.Visable && enemy.ReadyForDeletion == false)
                    {

                        if (_core.Actors.Player.Visable && _core.Actors.Player.ReadyForDeletion == false)
                        {
                            enemy.ApplyIntelligence(appliedOffset);

                            //Player collides with enemy.
                            if (enemy.Intersects(_core.Actors.Player))
                            {
                                _core.Actors.Player.Hit(enemy.CollisionDamage);
                                //enemy.Hit(enemy.CollisionDamage);
                            }

                            if (_core.Actors.Player.CurrentWeapon != null)
                            {
                                _core.Actors.Player.CurrentWeapon.ApplyIntelligence(appliedOffset, enemy); //Player lock-on to enemy. :D
                            }

                            lock (_core.Actors.Bullets)
                            {
                                foreach (var bullet in _core.Actors.Bullets)
                                {
                                    bullet.ApplyIntelligence(appliedOffset, enemy);
                                }
                            }
                        }

                        enemy.ApplyMotion(appliedOffset);
                    }
                }
            }

            #endregion

            #region Bullet Frame Advancement.

            lock (_core.Actors.Bullets)
            {
                foreach (var bullet in _core.Actors.Bullets)
                {
                    if (bullet.Visable && bullet.ReadyForDeletion == false)
                    {
                        bullet.ApplyIntelligence(appliedOffset, _core.Actors.Player);
                        bullet.ApplyMotion(appliedOffset);
                    }
                }
            }

            #endregion

            #region Stars Frame Advancement.

            if (appliedOffset.X != 0 || appliedOffset.Y != 0)
            {
                lock (_core.Actors.Stars)
                {
                    if (_core.Actors.Stars.Count < 100) //Never wan't more than n stars.
                    {
                        if (appliedOffset.X > 0)
                        {
                            for (int i = 0; i < 100; i++) //n chances to create a star.
                            {
                                if (Utility.Random.Next(0, 1000) == 500) //1 in n chance to create a star.
                                {
                                    int x = Utility.Random.Next(_core.Display.VisibleSize.Width - (int)appliedOffset.X, _core.Display.VisibleSize.Width);
                                    int y = Utility.Random.Next(0, _core.Display.VisibleSize.Height);
                                    _core.Actors.CreateStar(x, y);
                                }
                            }
                        }
                        else if (appliedOffset.X < 0)
                        {
                            for (int i = 0; i < 100; i++) //n chances to create a star.
                            {
                                if (Utility.Random.Next(0, 1000) == 500) //1 in n chance to create a star.
                                {
                                    int x = Utility.Random.Next(0, (int)-appliedOffset.X);
                                    int y = Utility.Random.Next(0, _core.Display.VisibleSize.Height);
                                    _core.Actors.CreateStar(x, y);
                                }
                            }
                        }

                        if (appliedOffset.Y > 0)
                        {
                            for (int i = 0; i < 100; i++) //n chances to create a star.
                            {
                                if (Utility.Random.Next(0, 1000) == 500) //1 in n chance to create a star.
                                {
                                    int x = Utility.Random.Next(0, _core.Display.VisibleSize.Width);
                                    int y = Utility.Random.Next(_core.Display.VisibleSize.Height - (int)appliedOffset.Y, _core.Display.VisibleSize.Height);
                                    _core.Actors.CreateStar(x, y);
                                }
                            }
                        }
                        else if (appliedOffset.Y < 0)
                        {
                            for (int i = 0; i < 100; i++) //n chances to create a star.
                            {
                                if (Utility.Random.Next(0, 1000) == 500) //1 in n chance to create a star.
                                {
                                    int x = Utility.Random.Next(0, _core.Display.VisibleSize.Width);
                                    int y = Utility.Random.Next(0, (int)-appliedOffset.Y);
                                    _core.Actors.CreateStar(x, y);
                                }
                            }
                        }
                    }

                    foreach (var star in _core.Actors.Stars)
                    {
                        if (_core.Display.VisibleBounds.IntersectsWith(star.Bounds) == false) //Remove count off-screen stars.
                        {
                            star.ReadyForDeletion = true;
                        }

                        star.X -= appliedOffset.X;
                        star.Y -= appliedOffset.Y;
                    }
                }
            }

            #endregion

            #region Animation Frame Advancement.

            lock (_core.Actors.Animations)
            {
                foreach (var animation in _core.Actors.Animations)
                {
                    if (animation.Visable)
                    {
                        animation.X += (animation.Velocity.Angle.X * (animation.Velocity.MaxSpeed * animation.Velocity.ThrottlePercentage)) - appliedOffset.X;
                        animation.Y += (animation.Velocity.Angle.Y * (animation.Velocity.MaxSpeed * animation.Velocity.ThrottlePercentage)) - appliedOffset.Y;
                        animation.AdvanceImage();
                    }
                }
            }

            #endregion

            #region Cleanup (cant be done in a foreach).

            lock (_core.Actors.EngineEvents)
            {
                for (int i = 0; i < _core.Actors.EngineEvents.Count; i++)
                {
                    if (_core.Actors.EngineEvents[i].ReadyForDeletion)
                    {
                        _core.Actors.DeleteEngineCallbackEvent(_core.Actors.EngineEvents[i]);
                    }
                }
            }

            lock (_core.Actors.Enemies)
            {
                for (int i = 0; i < _core.Actors.Enemies.Count; i++)
                {
                    if (_core.Actors.Enemies[i].ReadyForDeletion)
                    {
                        _core.Actors.DeleteEnemy(_core.Actors.Enemies[i]);
                    }
                }
            }

            lock (_core.Actors.Bullets)
            {
                for (int i = 0; i < _core.Actors.Bullets.Count; i++)
                {
                    if (_core.Actors.Bullets[i].ReadyForDeletion)
                    {
                        _core.Actors.DeleteBullet(_core.Actors.Bullets[i]);
                    }
                }
            }

            lock (_core.Actors.Animations)
            {
                for (int i = 0; i < _core.Actors.Animations.Count; i++)
                {
                    if (_core.Actors.Animations[i].ReadyForDeletion)
                    {
                        _core.Actors.DeleteAnimation(_core.Actors.Animations[i]);
                    }
                }
            }

            lock (_core.Actors.Stars)
            {
                for (int i = 0; i < _core.Actors.Stars.Count; i++)
                {
                    if (_core.Actors.Stars[i].ReadyForDeletion)
                    {
                        _core.Actors.DeleteStar(_core.Actors.Stars[i]);
                    }
                }
            }

            lock (_core.Actors.Player)
            {
                if (_core.Actors.Player.ReadyForDeletion)
                {
                    _core.Actors.Player.Cleanup();
                }
            }

            #endregion

            _core.Actors.PlayerStatsText.Text = $"HP: {_core.Actors.Player.HitPoints}, "
                + $"Weapon: {_core.Actors.Player.CurrentWeapon?.Name} X{_core.Actors.Player.CurrentWeapon?.RoundQuantity}, "
                + $"Quadrant: {_core.Display.CurrentQuadrant.Key.X}:{_core.Display.CurrentQuadrant.Key.Y}";

#if DEBUG
            if (_core.Actors.Debugs.Count > 0 && false)
            {
                var pointRight = Utility.AngleFromPointAtDistance(_core.Actors.Player.Velocity.Angle + 90, new PointD(50, 50));
                _core.Actors.Debugs[0].Location = _core.Actors.Player.Location + pointRight;
                _core.Actors.Debugs[0].Velocity.Angle = _core.Actors.Player.Velocity.Angle;

                var pointLeft = Utility.AngleFromPointAtDistance(_core.Actors.Player.Velocity.Angle - 90, new PointD(50, 50));
                _core.Actors.Debugs[1].Location = _core.Actors.Player.Location + pointLeft;
                _core.Actors.Debugs[1].Velocity.Angle = _core.Actors.Player.Velocity.Angle;
            }
#endif
        }
    }
}
