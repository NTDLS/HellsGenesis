using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Engine
{
    public class EngineThread
    {
        private Core _core;
        private bool _shutdown = false;
        private System.Threading.Thread _graphicsThread;
        private double _ramppedPlayerThrust = 0;

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

        private void GraphicsThreadProc()
        {
            while (_shutdown == false)
            {
                AdvanceFrame();
                System.Threading.Thread.Sleep(10);
            }
        }

        void AdvanceFrame()
        {
            _core.Display.FrameCounter.Calculate();

            #region Player Frame Advancement.

            if (_core.Input.IsKeyPressed(PlayerKey.Fire))
            {
                _core.Actors.Player.FireGun();
            }

            double wallWidth = 200; //Where "infinite scrolling" begins.

            double bgAppliedOffsetX = 0;
            double bgAppliedOffsetY = 0;

            //Make player thrust "build up" and fade.
            if (_core.Input.IsKeyPressed(PlayerKey.Forward))
            {
                if (_ramppedPlayerThrust < _core.Actors.Player.Velocity.Speed)
                {
                    _ramppedPlayerThrust += Consants.PlayerThrustRampUp;
                }
            }
            else if (_core.Input.IsKeyPressed(PlayerKey.Reverse))
            {
                if (_ramppedPlayerThrust > -_core.Actors.Player.Velocity.Speed)
                {
                    _ramppedPlayerThrust -= Consants.PlayerThrustRampUp;
                }
            }
            else
            {
                //If no "forward" or "reverse" user input is received... then fade the thrust.
                if (_ramppedPlayerThrust > 0)
                {
                    _ramppedPlayerThrust -= Consants.PlayerThrustRampDown;
                    if (_ramppedPlayerThrust < 0)
                    {
                        _ramppedPlayerThrust = 0; //We we overshot the fade, just stop the player.
                    }
                }
                else if (_ramppedPlayerThrust < 0)
                {
                    _ramppedPlayerThrust += Consants.PlayerThrustRampDown;
                    if (_ramppedPlayerThrust > 0)
                    {
                        _ramppedPlayerThrust = 0; //We we overshot the fade, just stop the player.
                    }
                }
            }

            if (_ramppedPlayerThrust > 0)
            {
                double forwardThrust = _ramppedPlayerThrust;

                //Close to the right wall and travelling in that direction.
                if (_core.Actors.Player.X > _core.Display.VisibleSize.Width - (_core.Actors.Player.Size.Width + wallWidth)
                    && _core.Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = (_core.Actors.Player.Velocity.Angle.X * forwardThrust);
                }

                //Close to the bottom wall and travelling in that direction.
                if (_core.Actors.Player.Y > _core.Display.VisibleSize.Height - (_core.Actors.Player.Size.Height + wallWidth)
                    && _core.Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = (_core.Actors.Player.Velocity.Angle.Y * forwardThrust);
                }

                //Close to the left wall and travelling in that direction.
                if (_core.Actors.Player.X < wallWidth && _core.Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = (_core.Actors.Player.Velocity.Angle.X * forwardThrust);
                }

                //Close to the top wall and travelling in that direction.
                if (_core.Actors.Player.Y < wallWidth && _core.Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = (_core.Actors.Player.Velocity.Angle.Y * forwardThrust);
                }

                _core.Actors.Player.X += (_core.Actors.Player.Velocity.Angle.X * forwardThrust) - bgAppliedOffsetX;
                _core.Actors.Player.Y += (_core.Actors.Player.Velocity.Angle.Y * forwardThrust) - bgAppliedOffsetY;

                _core.Actors.ShipEngineRoarSound.Play();
            }
            else if (_ramppedPlayerThrust < 0)
            {
                //Do we really need to reverse? This is space!

                double reverseThrust = -_ramppedPlayerThrust;

                //Close to the right wall and travelling in that direction.
                if (_core.Actors.Player.X > _core.Display.VisibleSize.Width - (_core.Actors.Player.Size.Width + wallWidth)
                    && _core.Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = -(_core.Actors.Player.Velocity.Angle.X * reverseThrust);
                }

                //Close to the bottom wall and travelling in that direction.
                if (_core.Actors.Player.Y > _core.Display.VisibleSize.Height - (_core.Actors.Player.Size.Height + wallWidth)
                    && _core.Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = -(_core.Actors.Player.Velocity.Angle.Y * reverseThrust);
                }

                //Close to the left wall and travelling in that direction.
                if (_core.Actors.Player.X < wallWidth && _core.Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = -(_core.Actors.Player.Velocity.Angle.X * reverseThrust);
                }

                //Close to the top wall and travelling in that direction.
                if (_core.Actors.Player.Y < wallWidth && _core.Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = -(_core.Actors.Player.Velocity.Angle.Y * reverseThrust);
                }

                _core.Actors.Player.X -= (_core.Actors.Player.Velocity.Angle.X * reverseThrust) + bgAppliedOffsetX;
                _core.Actors.Player.Y -= (_core.Actors.Player.Velocity.Angle.Y * reverseThrust) + bgAppliedOffsetY;

                _core.Actors.ShipEngineRoarSound.Play();
            }
            else
            {
                _core.Actors.ShipEngineRoarSound.Fade();
            }

            //Scroll the background.
            _core.Display.BackgroundOffset.X += bgAppliedOffsetX;
            _core.Display.BackgroundOffset.Y += bgAppliedOffsetY;

            if (_core.Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                _core.Actors.Player.Rotate(-_core.Actors.Player.RotationSpeed);
            }
            else if (_core.Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                _core.Actors.Player.Rotate(_core.Actors.Player.RotationSpeed);
            }

            #endregion

            #region Quadrant Math.

            _core.Display.CurrentView = new RectangleF(
                (float)_core.Display.BackgroundOffset.X,
                (float)_core.Display.BackgroundOffset.Y,
                (float)_core.Display.VisibleSize.Width,
                (float)_core.Display.VisibleSize.Height
            );

            _core.Display.CurrentQuadrant = _core.Display.GetQuadrant(
                _core.Actors.Player.X + _core.Display.BackgroundOffset.X,
                _core.Actors.Player.Y + _core.Display.BackgroundOffset.Y);

            _core.Actors.QuadrantText.Text =
                  $"       Frame Rate: Avg: {_core.Display.FrameCounter.AverageFrameRate.ToString("0.0")},"
                                   + $"Min: {_core.Display.FrameCounter.FrameRateMin.ToString("0.0")},"
                                   + $"Max: {_core.Display.FrameCounter.FrameRateMax.ToString("0.0")}\r\n"
                + $"Player Display XY: {_core.Actors.Player.X.ToString("#0.00")}x, {_core.Actors.Player.Y.ToString("#0.00")}y\r\n"
                + $"Player Virtual XY: {(_core.Actors.Player.X + _core.Display.BackgroundOffset.X).ToString("#0.00")}x,"
                                  + $" {(_core.Actors.Player.Y + _core.Display.BackgroundOffset.Y).ToString("#0.00")}y\r\n"
                + $"        BG Offset: {_core.Display.BackgroundOffset.X.ToString("#0.00")}x, {_core.Display.BackgroundOffset.Y.ToString("#0.00")}y\r\n"
                + $"         Quadrant: {_core.Display.CurrentQuadrant.Key.X}x, {_core.Display.CurrentQuadrant.Key.Y}y\r\n"
                + $"      Quadrant XY: {_core.Display.CurrentQuadrant.Bounds.X}x, {_core.Display.CurrentQuadrant.Bounds.Y}y\r\n"
                + $"    Quadrant Size: {_core.Display.CurrentQuadrant.Bounds.Width}x, {_core.Display.CurrentQuadrant.Bounds.Height}y";

            #endregion

            #region Enemies Frame Advancement.

            lock (_core.Actors.Enemies)
            {
                foreach (var enemy in _core.Actors.Enemies)
                {
                    if (_core.Actors.Player.Visable)
                    {
                        //double requiredAngle = enemy.RequiredAngleTo(_core.Actors.Player);
                        //_core.Actors.DebugText.Text = $"RA: {requiredAngle.ToString("####.###")}";

                        //double deltaAngle = Utility.GetDeltaAngle(enemy, _core.Actors.Player);
                        //_core.Actors.DebugText.Text = $"DA: {deltaAngle.ToString("####.###")}";

                        double distanceToPlayer = Utility.CalculeDistance(enemy, _core.Actors.Player);
                        if (distanceToPlayer < 500)
                        {
                            bool isPointingAtPlayer = enemy.IsPointingAt(_core.Actors.Player, 8.0);
                            if (isPointingAtPlayer)
                            {
                                enemy.FireGun();
                            }
                        }

                        if (enemy.X < (0 - (enemy.Size.Width + 40)) || enemy.Y < (0 - (enemy.Size.Height + 40))
                            || enemy.X >= (_core.Display.VisibleSize.Width + enemy.Size.Width) + 40
                            || enemy.Y >= (_core.Display.VisibleSize.Height + enemy.Size.Height) + 40)
                        {
                            enemy.MoveInDirectionOf(_core.Actors.Player);
                        }

                        if (enemy.Intersects(_core.Actors.Player))
                        {
                            _core.Actors.Player.Hit();
                        }
                    }

                    enemy.X += (enemy.Velocity.Angle.X * enemy.Velocity.Speed) - bgAppliedOffsetX;
                    enemy.Y += (enemy.Velocity.Angle.Y * enemy.Velocity.Speed) - bgAppliedOffsetY;

                    lock (_core.Actors.Bullets)
                    {
                        foreach (var bullet in _core.Actors.Bullets)
                        {
                            if (bullet.FiredFromType == FiredFromType.Player)
                            {
                                if (bullet.Intersects(enemy))
                                {
                                    enemy.Hit();
                                    bullet.ReadyForDeletion = true;
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Bullet Frame Advancement.

            lock (_core.Actors.Bullets)
            {
                foreach (var bullet in _core.Actors.Bullets)
                {
                    if (bullet.X < 0)
                    {
                        bullet.ReadyForDeletion = true;
                    }
                    else if (bullet.X >= _core.Display.VisibleSize.Width)
                    {
                        bullet.ReadyForDeletion = true;
                    }

                    if (bullet.Y < 0)
                    {
                        bullet.ReadyForDeletion = true;
                    }
                    else if (bullet.Y >= _core.Display.VisibleSize.Height)
                    {
                        bullet.ReadyForDeletion = true;
                    }

                    bullet.X += (bullet.Velocity.Angle.X * bullet.Velocity.Speed);
                    bullet.Y += (bullet.Velocity.Angle.Y * bullet.Velocity.Speed);

                    if (bullet.FiredFromType == FiredFromType.Enemy)
                    {
                        if (bullet.Intersects(_core.Actors.Player))
                        {
                            _core.Actors.Player.Hit();
                            bullet.ReadyForDeletion = true;
                        }
                    }
                }
            }

            #endregion

            #region Stars Frame Advancement.

            if (_core.Display.CurrentQuadrant.IsBackgroundPopulated == false)
            {
                _core.Display.CurrentQuadrant.IsBackgroundPopulated = true;

                for (int i = 0; i < 10; i++)
                {
                    _core.Actors.CreateStar(_core.Display.CurrentQuadrant);
                }
            }

            if (bgAppliedOffsetX != 0 || bgAppliedOffsetY != 0)
            {
                lock (_core.Actors.Stars)
                {
                    foreach (var star in _core.Actors.Stars)
                    {
                        star.X -= bgAppliedOffsetX;
                        star.Y -= bgAppliedOffsetY;
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
                        animation.X += (animation.Velocity.Angle.X * animation.Velocity.Speed) - bgAppliedOffsetX;
                        animation.Y += (animation.Velocity.Angle.Y * animation.Velocity.Speed) - bgAppliedOffsetY;
                        animation.AdvanceImage();
                    }
                }
            }

            #endregion

            #region Cleanup (cant be done in a foreach).

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

            /*
            lock (_core.Actors.Stars) //Do we really need to delete stars? I mean there are ALOT OF THEM!!!
            {
                for (int i = 0; i < _core.Actors.Animations.Count; i++)
                {
                    if (_core.Actors.Animations[i].ReadyForDeletion)
                    {
                        _core.Actors.DeleteAnimation(_core.Actors.Animations[i]);
                    }
                }
            }
            */

            lock (_core.Actors.Player)
            {
                if (_core.Actors.Player.ReadyForDeletion)
                {
                    _core.Actors.Player.Cleanup();
                }
            }

            #endregion

            //_core.Actors.DebugText.Text = $"{latestBackgroundRange.X.ToString("####.###")}x,{latestBackgroundRange.Y.ToString("####.###")}y"
            //    + $" x {latestBackgroundRange.Width.ToString("0000.000")}x,{latestBackgroundRange.Height.ToString("0000.000")}y";

            //_core.Actors.DebugText.Text = $"P: {_core.Actors.Player.X.ToString("000.00")},{_core.Actors.Player.Y.ToString("000.00")}"
            //    + $" B: {BackgroundOffset.X.ToString("000.00")},{BackgroundOffset.Y.ToString("000.00")}";

            //_core.Actors.DebugText.Text = $"View: {CurrentView.X.ToString("0000.000")}x, {CurrentView.Y.ToString("0000.000")}y"
            //    + $" x {CurrentView.Width.ToString("0000.000")}x, {CurrentView.Height.ToString("0000.000")}y";

            //_core.Actors.DebugText.Text = $" Q {CurrentQuadrant.Bounds.X}x, {CurrentQuadrant.Bounds.Y}y"
            //    + $" View: {CurrentView.X.ToString("0000.000")}x, {CurrentView.Y.ToString("0000.000")}y";

            //_core.Actors.PlayerStatsText.Text = $"HP: {_core.Actors.Player.HitPoints}, Ammo: {_core.Actors.Player.BulletsRemaining}";
        }
    }
}

