using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Engine
{
    public class GameThread
    {
        private Game _game;
        private bool _shutdown = false;
        private System.Threading.Thread _graphicsThread;

        public GameThread(Game game)
        {
            _game = game;

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
            _game.Display.FrameCounter.Calculate();

            #region Player Frame Advancement.

            if (_game.Input.IsKeyPressed(PlayerKey.Fire))
            {
                _game.Actors.Player.FireGun();
            }

            double wallWidth = 200;

            double bgAppliedOffsetX = 0;
            double bgAppliedOffsetY = 0;

            if (_game.Input.IsKeyPressed(PlayerKey.Forward))
            {
                //Close to the right wall and travelling in that direction.
                if (_game.Actors.Player.X > _game.Display.VisibleSize.Width - (_game.Actors.Player.Size.Width + wallWidth)
                    && _game.Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = (_game.Actors.Player.Velocity.Angle.X * _game.Actors.Player.Velocity.Speed);
                }

                //Close to the bottom wall and travelling in that direction.
                if (_game.Actors.Player.Y > _game.Display.VisibleSize.Height - (_game.Actors.Player.Size.Height + wallWidth)
                    && _game.Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = (_game.Actors.Player.Velocity.Angle.Y * _game.Actors.Player.Velocity.Speed);
                }

                //Close to the left wall and travelling in that direction.
                if (_game.Actors.Player.X < wallWidth && _game.Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = (_game.Actors.Player.Velocity.Angle.X * _game.Actors.Player.Velocity.Speed);
                }

                //Close to the top wall and travelling in that direction.
                if (_game.Actors.Player.Y < wallWidth && _game.Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = (_game.Actors.Player.Velocity.Angle.Y * _game.Actors.Player.Velocity.Speed);
                }

                _game.Actors.Player.X += (_game.Actors.Player.Velocity.Angle.X * _game.Actors.Player.Velocity.Speed) - bgAppliedOffsetX;
                _game.Actors.Player.Y += (_game.Actors.Player.Velocity.Angle.Y * _game.Actors.Player.Velocity.Speed) - bgAppliedOffsetY;

                _game.Actors.ShipEngineRoarSound.Play();
            }
            else if (_game.Input.IsKeyPressed(PlayerKey.Reverse))
            {
                //Close to the right wall and travelling in that direction.
                if (_game.Actors.Player.X > _game.Display.VisibleSize.Width - (_game.Actors.Player.Size.Width + wallWidth)
                    && _game.Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = -(_game.Actors.Player.Velocity.Angle.X * _game.Actors.Player.Velocity.Speed);
                }

                //Close to the bottom wall and travelling in that direction.
                if (_game.Actors.Player.Y > _game.Display.VisibleSize.Height - (_game.Actors.Player.Size.Height + wallWidth)
                    && _game.Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = -(_game.Actors.Player.Velocity.Angle.Y * _game.Actors.Player.Velocity.Speed);
                }

                //Close to the left wall and travelling in that direction.
                if (_game.Actors.Player.X < wallWidth && _game.Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = -(_game.Actors.Player.Velocity.Angle.X * _game.Actors.Player.Velocity.Speed);
                }

                //Close to the top wall and travelling in that direction.
                if (_game.Actors.Player.Y < wallWidth && _game.Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = -(_game.Actors.Player.Velocity.Angle.Y * _game.Actors.Player.Velocity.Speed);
                }

                _game.Actors.Player.X -= (_game.Actors.Player.Velocity.Angle.X * _game.Actors.Player.Velocity.Speed) + bgAppliedOffsetX;
                _game.Actors.Player.Y -= (_game.Actors.Player.Velocity.Angle.Y * _game.Actors.Player.Velocity.Speed) + bgAppliedOffsetY;

                _game.Actors.ShipEngineRoarSound.Play();
            }
            else
            {
                _game.Actors.ShipEngineRoarSound.Fade();
            }

            //Scroll the background.
            _game.Display.BackgroundOffset.X += bgAppliedOffsetX;
            _game.Display.BackgroundOffset.Y += bgAppliedOffsetY;

            if (_game.Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                _game.Actors.Player.Rotate(-_game.Actors.Player.RotationSpeed);
            }
            else if (_game.Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                _game.Actors.Player.Rotate(_game.Actors.Player.RotationSpeed);
            }

            #endregion

            #region Quadrant Math.

            _game.Display.CurrentView = new RectangleF(
                (float)_game.Display.BackgroundOffset.X,
                (float)_game.Display.BackgroundOffset.Y,
                (float)_game.Display.VisibleSize.Width,
                (float)_game.Display.VisibleSize.Height
            );

            _game.Display.CurrentQuadrant = _game.Display.GetQuadrant(
                _game.Actors.Player.X + _game.Display.BackgroundOffset.X,
                _game.Actors.Player.Y + _game.Display.BackgroundOffset.Y);

            _game.Actors.QuadrantText.Text =
                  $"       Frame Rate: Avg: {_game.Display.FrameCounter.AverageFrameRate.ToString("0.0")},"
                                   + $"Min: {_game.Display.FrameCounter.FrameRateMin.ToString("0.0")},"
                                   + $"Max: {_game.Display.FrameCounter.FrameRateMax.ToString("0.0")}\r\n"
                + $"Player Display XY: {_game.Actors.Player.X.ToString("#0.00")}x, {_game.Actors.Player.Y.ToString("#0.00")}y\r\n"
                + $"Player Virtual XY: {(_game.Actors.Player.X + _game.Display.BackgroundOffset.X).ToString("#0.00")}x,"
                                  + $" {(_game.Actors.Player.Y + _game.Display.BackgroundOffset.Y).ToString("#0.00")}y\r\n"
                + $"        BG Offset: {_game.Display.BackgroundOffset.X.ToString("#0.00")}x, {_game.Display.BackgroundOffset.Y.ToString("#0.00")}y\r\n"
                + $"         Quadrant: {_game.Display.CurrentQuadrant.Key.X}x, {_game.Display.CurrentQuadrant.Key.Y}y\r\n"
                + $"      Quadrant XY: {_game.Display.CurrentQuadrant.Bounds.X}x, {_game.Display.CurrentQuadrant.Bounds.Y}y\r\n"
                + $"    Quadrant Size: {_game.Display.CurrentQuadrant.Bounds.Width}x, {_game.Display.CurrentQuadrant.Bounds.Height}y";

            #endregion

            #region Enemies Frame Advancement.

            lock (_game.Actors.Enemies)
            {
                foreach (var enemy in _game.Actors.Enemies)
                {
                    if (_game.Actors.Player.Visable)
                    {
                        //double requiredAngle = enemy.RequiredAngleTo(_game.Actors.Player);
                        //_game.Actors.DebugText.Text = $"RA: {requiredAngle.ToString("####.###")}";

                        //double deltaAngle = Utility.GetDeltaAngle(enemy, _game.Actors.Player);
                        //_game.Actors.DebugText.Text = $"DA: {deltaAngle.ToString("####.###")}";

                        double distanceToPlayer = Utility.CalculeDistance(enemy, _game.Actors.Player);
                        if (distanceToPlayer < 500)
                        {
                            bool isPointingAtPlayer = enemy.IsPointingAt(_game.Actors.Player, 8.0);
                            if (isPointingAtPlayer)
                            {
                                enemy.FireGun();
                            }
                        }

                        if (enemy.X < (0 - (enemy.Size.Width + 40)) || enemy.Y < (0 - (enemy.Size.Height + 40))
                            || enemy.X >= (_game.Display.VisibleSize.Width + enemy.Size.Width) + 40
                            || enemy.Y >= (_game.Display.VisibleSize.Height + enemy.Size.Height) + 40)
                        {
                            enemy.MoveInDirectionOf(_game.Actors.Player);
                        }

                        if (enemy.Intersects(_game.Actors.Player))
                        {
                            _game.Actors.Player.Hit();
                        }
                    }

                    enemy.X += (enemy.Velocity.Angle.X * enemy.Velocity.Speed) - bgAppliedOffsetX;
                    enemy.Y += (enemy.Velocity.Angle.Y * enemy.Velocity.Speed) - bgAppliedOffsetY;

                    lock (_game.Actors.Bullets)
                    {
                        foreach (var bullet in _game.Actors.Bullets)
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

            lock (_game.Actors.Bullets)
            {
                foreach (var bullet in _game.Actors.Bullets)
                {
                    if (bullet.X < 0)
                    {
                        bullet.ReadyForDeletion = true;
                    }
                    else if (bullet.X >= _game.Display.VisibleSize.Width)
                    {
                        bullet.ReadyForDeletion = true;
                    }

                    if (bullet.Y < 0)
                    {
                        bullet.ReadyForDeletion = true;
                    }
                    else if (bullet.Y >= _game.Display.VisibleSize.Height)
                    {
                        bullet.ReadyForDeletion = true;
                    }

                    bullet.X += (bullet.Velocity.Angle.X * bullet.Velocity.Speed);
                    bullet.Y += (bullet.Velocity.Angle.Y * bullet.Velocity.Speed);

                    if (bullet.FiredFromType == FiredFromType.Enemy)
                    {
                        if (bullet.Intersects(_game.Actors.Player))
                        {
                            _game.Actors.Player.Hit();
                            bullet.ReadyForDeletion = true;
                        }
                    }
                }
            }

            #endregion

            #region Stars Frame Advancement.

            if (_game.Display.CurrentQuadrant.IsBackgroundPopulated == false)
            {
                _game.Display.CurrentQuadrant.IsBackgroundPopulated = true;

                for (int i = 0; i < 10; i++)
                {
                    _game.Actors.CreateStar(_game.Display.CurrentQuadrant);
                }
            }

            if (bgAppliedOffsetX != 0 || bgAppliedOffsetY != 0)
            {
                lock (_game.Actors.Stars)
                {
                    foreach (var star in _game.Actors.Stars)
                    {
                        star.X -= bgAppliedOffsetX;
                        star.Y -= bgAppliedOffsetY;
                    }
                }
            }

            #endregion

            #region Animation Frame Advancement.

            lock (_game.Actors.Animations)
            {
                foreach (var animation in _game.Actors.Animations)
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

            lock (_game.Actors.Enemies)
            {
                for (int i = 0; i < _game.Actors.Enemies.Count; i++)
                {
                    if (_game.Actors.Enemies[i].ReadyForDeletion)
                    {
                        _game.Actors.DeleteEnemy(_game.Actors.Enemies[i]);
                    }
                }
            }

            lock (_game.Actors.Bullets)
            {
                for (int i = 0; i < _game.Actors.Bullets.Count; i++)
                {
                    if (_game.Actors.Bullets[i].ReadyForDeletion)
                    {
                        _game.Actors.DeleteBullet(_game.Actors.Bullets[i]);
                    }
                }
            }

            lock (_game.Actors.Animations)
            {
                for (int i = 0; i < _game.Actors.Animations.Count; i++)
                {
                    if (_game.Actors.Animations[i].ReadyForDeletion)
                    {
                        _game.Actors.DeleteAnimation(_game.Actors.Animations[i]);
                    }
                }
            }

            /*
            lock (_game.Actors.Stars) //Do we really need to delete stars? I mean there are ALOT OF THEM!!!
            {
                for (int i = 0; i < _game.Actors.Animations.Count; i++)
                {
                    if (_game.Actors.Animations[i].ReadyForDeletion)
                    {
                        _game.Actors.DeleteAnimation(_game.Actors.Animations[i]);
                    }
                }
            }
            */

            lock (_game.Actors.Player)
            {
                if (_game.Actors.Player.ReadyForDeletion)
                {
                    _game.Actors.Player.Cleanup();
                }
            }

            #endregion

            //_game.Actors.DebugText.Text = $"{latestBackgroundRange.X.ToString("####.###")}x,{latestBackgroundRange.Y.ToString("####.###")}y"
            //    + $" x {latestBackgroundRange.Width.ToString("0000.000")}x,{latestBackgroundRange.Height.ToString("0000.000")}y";

            //_game.Actors.DebugText.Text = $"P: {_game.Actors.Player.X.ToString("000.00")},{_game.Actors.Player.Y.ToString("000.00")}"
            //    + $" B: {BackgroundOffset.X.ToString("000.00")},{BackgroundOffset.Y.ToString("000.00")}";

            //_game.Actors.DebugText.Text = $"View: {CurrentView.X.ToString("0000.000")}x, {CurrentView.Y.ToString("0000.000")}y"
            //    + $" x {CurrentView.Width.ToString("0000.000")}x, {CurrentView.Height.ToString("0000.000")}y";

            //_game.Actors.DebugText.Text = $" Q {CurrentQuadrant.Bounds.X}x, {CurrentQuadrant.Bounds.Y}y"
            //    + $" View: {CurrentView.X.ToString("0000.000")}x, {CurrentView.Y.ToString("0000.000")}y";

            //_game.Actors.PlayerStatsText.Text = $"HP: {_game.Actors.Player.HitPoints}, Ammo: {_game.Actors.Player.BulletsRemaining}";
        }


    }
}
