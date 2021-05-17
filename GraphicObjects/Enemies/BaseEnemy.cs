using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;

namespace AI2D.GraphicObjects.Enemies
{
    public class BaseEnemy : BaseGraphicObject
    {
        public int CollisionDamage { get; set; } = 25;
        public int ScorePoints { get; private set; } = 25;
        public ObjRadarPositionIndicator RadarPositionIndicator { get; set; }
        public ObjRadarPositionTextBlock RadarPositionText { get; set; }
        public ObjAnimation ThrustAnimation { get; private set; }

        public BaseEnemy(Core core, int hitPoints, int scoreMultiplier)
            : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            base.SetHitPoints(hitPoints);
            ScorePoints = HitPoints * scoreMultiplier;

            RadarPositionIndicator = _core.Actors.AddNewRadarPositionIndicator();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.AddNewRadarPositionTextBlock("Consolas", Brushes.Red, 8, new PointD());

            string _debugAniPath = @"..\..\Assets\Graphics\Animation\AirThrust32x32.png";
            var playMode = new ObjAnimation.PlayMode()
            {
                Replay = ObjAnimation.ReplayMode.LoopedPlay,
                DeleteActorAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };
            ThrustAnimation = new ObjAnimation(_core, _debugAniPath, new Size(32, 32), 10, playMode);

            ThrustAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(ThrustAnimation, this);

            var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new PointD(20, 20));
            ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
            ThrustAnimation.X = this.X + pointRight.X;
            ThrustAnimation.Y = this.Y + pointRight.Y;

            this.OnPositionChanged += BaseEnemy_OnPositionChanged;
            this.OnRotated += BaseEnemy_OnPositionChanged;
        }

        private void BaseEnemy_OnPositionChanged(BaseGraphicObject obj)
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new PointD(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = this.X + pointRight.X;
                ThrustAnimation.Y = this.Y + pointRight.Y;
            }
        }

        public static int GetGenericHP()
        {
            return Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);
        }

        public virtual void ApplyMotion(PointD frameAppliedOffset)
        {
            if (X < -Constants.Limits.EnemySceneDistanceLimit
                || X >= _core.Display.VisibleSize.Width + Constants.Limits.EnemySceneDistanceLimit
                || Y < -Constants.Limits.EnemySceneDistanceLimit
                || Y >= _core.Display.VisibleSize.Height + Constants.Limits.EnemySceneDistanceLimit)
            {
                QueueForDelete();;
                return;
            }

            X += (Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.X;
            Y += (Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.Y;

            if (RadarPositionIndicator != null)
            {
                if (X < 0 || X >= _core.Display.VisibleSize.Width || Y < 0 || Y >= _core.Display.VisibleSize.Height)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_core.Actors.Player));

                    RadarPositionText.Visable = true;
                    RadarPositionIndicator.Visable = true;

                    double requiredAngle = _core.Actors.Player.AngleTo(this);

                    var offset = Utility.AngleFromPointAtDistance(new AngleD(requiredAngle), new PointD(200, 200));

                    RadarPositionText.Location = _core.Actors.Player.Location + offset + new PointD(25, 25);
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;

                    RadarPositionIndicator.Location = _core.Actors.Player.Location + offset;
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;
                }
                else
                {
                    RadarPositionText.Visable = false;
                    RadarPositionIndicator.Visable = false;
                }
            }
        }

        public virtual void ApplyIntelligence(PointD frameAppliedOffset)
        {
            if (CurrentWeapon != null && _core.Actors.Player != null)
            {
                CurrentWeapon.ApplyIntelligence(frameAppliedOffset, _core.Actors.Player); //Enemy lock-on to Player. :O
            }
        }

        public override void Cleanup()
        {
            if (RadarPositionIndicator != null)
            {
                RadarPositionIndicator.QueueForDelete();;
                RadarPositionText.QueueForDelete();;
            }
            if (ThrustAnimation != null)
            {
                ThrustAnimation.QueueForDelete();;
            }
            base.Cleanup();
        }
    }
}