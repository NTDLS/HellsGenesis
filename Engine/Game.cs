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
        public GameInput Input { get; private set; }
        public GameDisplay Display { get; private set; }
        public GameActors Actors { get; private set; }
        public PointD BackgroundOffset { get; private set; } = new PointD(); //Offset of background, all cals must take into account.
        public RectangleF CurrentView { get; private set; } //Rectangle of the currently displayed coords.
        public Dictionary<Point, Quadrant> Quadrants = new Dictionary<Point, Quadrant>();
        public Quadrant CurrentQuadrant { get; private set; }

        public void Start()
        {
            Actors.BackgroundMusicSound.Play();

            for (int i = 0; i < 1; i++)
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
            Display = new GameDisplay(drawingSurface, visibleSize);
            Actors = new GameActors(this);
            Input = new GameInput(this);

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

        private DateTime _lastFrame = DateTime.MinValue;
        private double _currentFrameRate;
        private double _totalFrameRate;
        private double _averageFrameRate = double.PositiveInfinity;
        private int _frameRateSamples;
        private double _FrameRateMin = double.PositiveInfinity;
        private double _FrameRateMax = double.NegativeInfinity;

        void AdvanceFrame()
        {
            #region Player Frame Advancement.

            if (_lastFrame != DateTime.MinValue)
            {
                if (_frameRateSamples == 0 || _frameRateSamples > 1000)
                {
                    _frameRateSamples = 1;
                    _totalFrameRate = 0;
                }

                _currentFrameRate = 1000.0 / (DateTime.Now - _lastFrame).TotalMilliseconds;
                _totalFrameRate += _currentFrameRate;

                if (_frameRateSamples > 100)
                {
                    _FrameRateMin = _currentFrameRate < _FrameRateMin ? _currentFrameRate : _FrameRateMin;
                    _FrameRateMax = _currentFrameRate > _FrameRateMax ? _currentFrameRate : _FrameRateMax;
                    _averageFrameRate = _totalFrameRate / (double)_frameRateSamples;
                }
                _frameRateSamples++;
            }

            _lastFrame = DateTime.Now;

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
            BackgroundOffset.X += bgAppliedOffsetX;
            BackgroundOffset.Y += bgAppliedOffsetY;

            if (Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                Actors.Player.Rotate(-Actors.Player.RotationSpeed);
            }
            else if (Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                Actors.Player.Rotate(Actors.Player.RotationSpeed);
            }

            #endregion

            #region Quadrant Math.

            CurrentView = new RectangleF(
                (float)BackgroundOffset.X,
                (float)BackgroundOffset.Y,
                (float)Display.VisibleSize.Width,
                (float)Display.VisibleSize.Height
            );

            CurrentQuadrant = GetQuadrant(Actors.Player.X + BackgroundOffset.X, Actors.Player.Y + BackgroundOffset.Y);

            Actors.QuadrantText.Text =
                  $"       Frame Rate: Avg: {_averageFrameRate.ToString("0.0")}, Min: {_FrameRateMin.ToString("0.0")}, Max: {_FrameRateMax.ToString("0.0")}\r\n"
                + $"Player Display XY: {Actors.Player.X.ToString("#0.00")}x, {Actors.Player.Y.ToString("#0.00")}y\r\n"
                + $"Player Virtual XY: {(Actors.Player.X + BackgroundOffset.X).ToString("#0.00")}x, {(Actors.Player.Y + BackgroundOffset.Y).ToString("#0.00")}y\r\n"
                + $"        BG Offset: {BackgroundOffset.X.ToString("#0.00")}x, {BackgroundOffset.Y.ToString("#0.00")}y\r\n"
                + $"         Quadrant: {CurrentQuadrant.Key.X}x, {CurrentQuadrant.Key.Y}y\r\n"
                + $"      Quadrant XY: {CurrentQuadrant.Bounds.X}x, {CurrentQuadrant.Bounds.Y}y\r\n"
                + $"    Quadrant Size: {CurrentQuadrant.Bounds.Width}x, {CurrentQuadrant.Bounds.Height}y";

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

            if (CurrentQuadrant.IsBackgroundPopulated == false)
            {
                CurrentQuadrant.IsBackgroundPopulated = true;

                for (int i = 0; i < 10; i++)
                {
                    Actors.CreateStar(CurrentQuadrant);
                }
            }

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

            //Actors.DebugText.Text = $"{latestBackgroundRange.X.ToString("####.###")}x,{latestBackgroundRange.Y.ToString("####.###")}y"
            //    + $" x {latestBackgroundRange.Width.ToString("0000.000")}x,{latestBackgroundRange.Height.ToString("0000.000")}y";

            //Actors.DebugText.Text = $"P: {Actors.Player.X.ToString("000.00")},{Actors.Player.Y.ToString("000.00")}"
            //    + $" B: {BackgroundOffset.X.ToString("000.00")},{BackgroundOffset.Y.ToString("000.00")}";

            //Actors.DebugText.Text = $"View: {CurrentView.X.ToString("0000.000")}x, {CurrentView.Y.ToString("0000.000")}y"
            //    + $" x {CurrentView.Width.ToString("0000.000")}x, {CurrentView.Height.ToString("0000.000")}y";

            //Actors.DebugText.Text = $" Q {CurrentQuadrant.Bounds.X}x, {CurrentQuadrant.Bounds.Y}y"
            //    + $" View: {CurrentView.X.ToString("0000.000")}x, {CurrentView.Y.ToString("0000.000")}y";

            //Actors.PlayerStatsText.Text = $"HP: {Actors.Player.HitPoints}, Ammo: {Actors.Player.BulletsRemaining}";
        }

        public void RenderObjects(Graphics dc)
        {
            Actors.RenderObjects(dc);
        }

        #endregion
        
        public Quadrant GetQuadrant(double x, double y)
        {
            var coord = new Point(
                    (int)(x / Display.VisibleSize.Width),
                    (int)(y / Display.VisibleSize.Height)
                );

            if (Quadrants.ContainsKey(coord) == false)
            {
                var absoluteBounds = new Rectangle(
                    Display.VisibleSize.Width * coord.X,
                    Display.VisibleSize.Height * coord.Y,
                    Display.VisibleSize.Width,
                    Display.VisibleSize.Height);

                var quad = new Quadrant(coord, absoluteBounds);

                Quadrants.Add(coord, quad);
            }

            return Quadrants[coord];
        }
    }
}