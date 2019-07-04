using AI2D.Objects;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class Game
    {
        private bool _shutdown = false;
        private System.Threading.Thread _graphicsThread;
        public UserInput Input { get; private set; }
        public Display Display { get; private set; }
        public ActorAssets Actors { get; private set; }
        public PointD _backgroundOffset { get; set; } = new PointD();
        private RectangleD _renderedBackgroundArea { get; set; } = new RectangleD();
        public RectangleF CurrentView { get; private set; }

        public void Start()
        {
            Actors.BackgroundMusicSound.Play();

            _renderedBackgroundArea = new RectangleD(0, 0, Display.VisibleSize.Width, Display.VisibleSize.Height);

            for (int i = 0; i < 100; i++)
            {
                Actors.CreateStar(); // (100, 100);
            }

            for (int i = 0; i < 10; i++)
            {
                Actors.CreateEnemy();
            }

            _shutdown = false;
            Actors.ShowNewPlayer();

            _graphicsThread.Start();
        }

        public void Stop()
        {
            _shutdown = true;
        }

        #region ~/ctor.

        public Game(Control drawingSurface, Size visibleSize)
        {
            Display = new Display(drawingSurface, visibleSize);
            Actors = new ActorAssets(this);
            Input = new UserInput(this);

            _graphicsThread = new System.Threading.Thread(GraphicsThreadProc);
        }

        #endregion

        #region Engine Graphics.

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
            #region Player Frame Advancement.

            if (Input.IsKeyPressed(PlayerKey.Fire))
            {
                Actors.Player.FireGun();
            }

            double wallWidth = 200;

            double bgAppliedOffsetX = 0;
            double bgAppliedOffsetY = 0;

            if (Input.IsKeyPressed(PlayerKey.Forward))
            {
                //Close to the right wall and travelling in that direction.
                if (Actors.Player.X > Display.VisibleSize.Width - (Actors.Player.Size.Width + wallWidth) && Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the bottom wall and travelling in that direction.
                if (Actors.Player.Y > Display.VisibleSize.Height - (Actors.Player.Size.Height + wallWidth) && Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                //Close to the left wall and travelling in that direction.
                if (Actors.Player.X < wallWidth && Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the top wall and travelling in that direction.
                if (Actors.Player.Y < wallWidth && Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                Actors.Player.X += (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed) - bgAppliedOffsetX;
                Actors.Player.Y += (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed) - bgAppliedOffsetY;
                Actors.ShipEngineRoarSound.Play();
            }
            else if (Input.IsKeyPressed(PlayerKey.Reverse))
            {
                //Close to the right wall and travelling in that direction.
                if (Actors.Player.X > Display.VisibleSize.Width - (Actors.Player.Size.Width + wallWidth) && Actors.Player.Velocity.Angle.X < 0)
                {
                    bgAppliedOffsetX = -(Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the bottom wall and travelling in that direction.
                if (Actors.Player.Y > Display.VisibleSize.Height - (Actors.Player.Size.Height + wallWidth) && Actors.Player.Velocity.Angle.Y < 0)
                {
                    bgAppliedOffsetY = -(Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                //Close to the left wall and travelling in that direction.
                if (Actors.Player.X < wallWidth && Actors.Player.Velocity.Angle.X > 0)
                {
                    bgAppliedOffsetX = -(Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed);
                }

                //Close to the top wall and travelling in that direction.
                if (Actors.Player.Y < wallWidth && Actors.Player.Velocity.Angle.Y > 0)
                {
                    bgAppliedOffsetY = -(Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed);
                }

                Actors.Player.X -= (Actors.Player.Velocity.Angle.X * Actors.Player.Velocity.Speed) + bgAppliedOffsetX;
                Actors.Player.Y -= (Actors.Player.Velocity.Angle.Y * Actors.Player.Velocity.Speed) + bgAppliedOffsetY;
                Actors.ShipEngineRoarSound.Play();
            }
            else
            {
                Actors.ShipEngineRoarSound.Fade();
            }

            //Scroll the background.
            _backgroundOffset.X += bgAppliedOffsetX;
            _backgroundOffset.Y += bgAppliedOffsetY;

            //Keep track of the areas for which we have rendered background.
            var latestBackgroundRange = new RectangleD()
            {
                X = _backgroundOffset.X < _renderedBackgroundArea.X ? _backgroundOffset.X : _renderedBackgroundArea.X,
                Y = _backgroundOffset.Y < _renderedBackgroundArea.Y ? _backgroundOffset.Y : _renderedBackgroundArea.Y,
                Width = _backgroundOffset.X + Display.VisibleSize.Width > _renderedBackgroundArea.Width ? _backgroundOffset.X + Display.VisibleSize.Width : _renderedBackgroundArea.Width,
                Height = _backgroundOffset.Y + Display.VisibleSize.Height > _renderedBackgroundArea.Height ? _backgroundOffset.Y + Display.VisibleSize.Height : _renderedBackgroundArea.Width
            };

            if (Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                Actors.Player.Rotate(-Actors.Player.RotationSpeed);
            }
            else if (Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                Actors.Player.Rotate(Actors.Player.RotationSpeed);
            }

            #endregion

            #region Enemies Frame Advancement.

            lock (Actors.Enemies)
            {
                foreach (var enemy in Actors.Enemies)
                {
                    if (Actors.Player.Visable)
                    {
                        //double requiredAngle = enemy.RequiredAngleTo(Actors.Player);
                        //Actors.DebugText.Text = $"RA: {requiredAngle.ToString("####.###")}";

                        //double deltaAngle = Utility.GetDeltaAngle(enemy, Actors.Player);
                        //Actors.DebugText.Text = $"DA: {deltaAngle.ToString("####.###")}";

                        double distanceToPlayer = Utility.CalculeDistance(enemy, Actors.Player);
                        if (distanceToPlayer < 500)
                        {
                            bool isPointingAtPlayer = enemy.IsPointingAt(Actors.Player, 8.0);
                            if (isPointingAtPlayer)
                            {
                                enemy.FireGun();
                            }
                        }

                        if (enemy.X < (0 - (enemy.Size.Width + 40)) || enemy.Y < (0 - (enemy.Size.Height + 40))
                            || enemy.X >= (Display.VisibleSize.Width + enemy.Size.Width) + 40
                            || enemy.Y >= (Display.VisibleSize.Height + enemy.Size.Height) + 40)
                        {
                            enemy.MoveInDirectionOf(Actors.Player);
                        }

                        if (enemy.Intersects(Actors.Player))
                        {
                            Actors.Player.Hit();
                        }
                    }

                    enemy.X += (enemy.Velocity.Angle.X * enemy.Velocity.Speed) - bgAppliedOffsetX;
                    enemy.Y += (enemy.Velocity.Angle.Y * enemy.Velocity.Speed) - bgAppliedOffsetY;

                    lock (Actors.Bullets)
                    {
                        foreach (var bullet in Actors.Bullets)
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

            lock (Actors.Bullets)
            {
                foreach (var bullet in Actors.Bullets)
                {
                    if (bullet.X < 0)
                    {
                        bullet.ReadyForDeletion = true;
                    }
                    else if (bullet.X >= Display.VisibleSize.Width)
                    {
                        bullet.ReadyForDeletion = true;
                    }

                    if (bullet.Y < 0)
                    {
                        bullet.ReadyForDeletion = true;
                    }
                    else if (bullet.Y >= Display.VisibleSize.Height)
                    {
                        bullet.ReadyForDeletion = true;
                    }

                    bullet.X += (bullet.Velocity.Angle.X * bullet.Velocity.Speed);
                    bullet.Y += (bullet.Velocity.Angle.Y * bullet.Velocity.Speed);

                    if (bullet.FiredFromType == FiredFromType.Enemy)
                    {
                        if (bullet.Intersects(Actors.Player))
                        {
                            Actors.Player.Hit();
                            bullet.ReadyForDeletion = true;
                        }
                    }
                }
            }

            #endregion

            #region Stars Frame Advancement.

            if (bgAppliedOffsetX != 0 || bgAppliedOffsetY != 0)
            {
                lock (Actors.Stars)
                {
                    foreach (var star in Actors.Stars)
                    {
                        star.X -= bgAppliedOffsetX;
                        star.Y -= bgAppliedOffsetY;
                    }
                }

                double deltaX = _renderedBackgroundArea.X - latestBackgroundRange.X;
                double deltaY = _renderedBackgroundArea.Y - latestBackgroundRange.Y;
                double deltaWidth = _renderedBackgroundArea.Width - latestBackgroundRange.Width;
                double deltaHeight = _renderedBackgroundArea.Height - latestBackgroundRange.Height;

                if (deltaX != 0)
                {
                    deltaX -= Display.VisibleSize.Width;

                    double size = Math.Abs(latestBackgroundRange.Height * deltaX);

                    for (int i = 0; i < 100; i++)
                    {
                        double x = Utility.Random.Next(0, (int)Math.Abs(deltaX)) + deltaX;
                        double y = Utility.Random.Next(0, (int)latestBackgroundRange.Height);
                        //Actors.CreateStar(x, y);
                    }

                    latestBackgroundRange.X = latestBackgroundRange.X - Display.VisibleSize.Width;
                }

                if (deltaY != 0)
                {
                    deltaY -= Display.VisibleSize.Height;

                    double size = Math.Abs(latestBackgroundRange.Width * deltaY);

                    for (int i = 0; i < 100; i++)
                    {
                        double x = Utility.Random.Next(0, (int)latestBackgroundRange.Width);
                        double y = Utility.Random.Next(0, (int)Math.Abs(deltaY)) + deltaY;
                        //Actors.CreateStar(x, y);
                    }

                    latestBackgroundRange.Y = latestBackgroundRange.Y - Display.VisibleSize.Height;
                }
            }

            #endregion

            #region Animation Frame Advancement.

            lock (Actors.Animations)
            {
                foreach (var animation in Actors.Animations)
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

            lock (Actors.Enemies)
            {
                for (int i = 0; i < Actors.Enemies.Count; i++)
                {
                    if (Actors.Enemies[i].ReadyForDeletion)
                    {
                        Actors.DeleteEnemy(Actors.Enemies[i]);
                    }
                }
            }

            lock (Actors.Bullets)
            {
                for (int i = 0; i < Actors.Bullets.Count; i++)
                {
                    if (Actors.Bullets[i].ReadyForDeletion)
                    {
                        Actors.DeleteBullet(Actors.Bullets[i]);
                    }
                }
            }

            lock (Actors.Animations)
            {
                for (int i = 0; i < Actors.Animations.Count; i++)
                {
                    if (Actors.Animations[i].ReadyForDeletion)
                    {
                        Actors.DeleteAnimation(Actors.Animations[i]);
                    }
                }
            }

            /*
            lock (Actors.Stars) //Do we really need to delete stars? I mean there are ALOT OF THEM!!!
            {
                for (int i = 0; i < Actors.Animations.Count; i++)
                {
                    if (Actors.Animations[i].ReadyForDeletion)
                    {
                        Actors.DeleteAnimation(Actors.Animations[i]);
                    }
                }
            }
            */

            lock (Actors.Player)
            {
                if (Actors.Player.ReadyForDeletion)
                {
                    Actors.Player.Cleanup();
                }
            }

            #endregion

            _renderedBackgroundArea = latestBackgroundRange;

            //Actors.DebugBlock.Text = $"{latestBackgroundRange.X.ToString("####.###")}x,{latestBackgroundRange.Y.ToString("####.###")}y"
            //    + $" x {latestBackgroundRange.Width.ToString("####.###")}x,{latestBackgroundRange.Height.ToString("####.###")}y";

            //Actors.DebugBlock.Text = $"P: {Actors.Player.X.ToString("####.###")},{Actors.Player.Y.ToString("####.###")}"
            //    + $" B: {_backgroundOffset.X.ToString("####.###")},{_backgroundOffset.Y.ToString("####.###")}";

            CurrentView = new RectangleF(
                (float)_backgroundOffset.X,
                (float)_backgroundOffset.Y,
                (float)Display.VisibleSize.Width,
                (float)Display.VisibleSize.Height
            );                

            //Actors.DebugText.Text = $"View: {CurrentView.X.ToString("####.###")}x, {CurrentView.Y.ToString("####.###")}y"
            //    + $" x {CurrentView.Width.ToString("####.###")}x, {CurrentView.Height.ToString("####.###")}y";

            Actors.PlayerStatsText.Text = $"HP: {Actors.Player.HitPoints}, Ammo: {Actors.Player.BulletsRemaining}";
        }

        public void RenderObjects(Graphics dc)
        {
            Actors.RenderObjects(dc);
        }

        #endregion
    }
}
