using AI2D.Engine;
using AI2D.Types;
using Determinet;
using System;
using System.Collections.Generic;
using System.Drawing;
using static AI2D.Actors.Enemies.AI.BrainBase;

namespace AI2D.Actors.Enemies
{
    public class EnemyBase : ActorBase
    {
        public int CollisionDamage { get; set; } = 25;
        public int ScorePoints { get; private set; } = 25;
        public ActorRadarPositionIndicator RadarPositionIndicator { get; set; }
        public ActorRadarPositionTextBlock RadarPositionText { get; set; }
        public ActorAnimation ThrustAnimation { get; private set; }

        public EnemyBase(Core core, int hitPoints, int scoreMultiplier)
            : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            RadarDotSize = new Point<int>(4, 4);
            RadarDotColor = Color.FromArgb(200, 100, 100);

            base.SetHitPoints(hitPoints);
            ScorePoints = HitPoints * scoreMultiplier;

            RadarPositionIndicator = _core.Actors.AddNewRadarPositionIndicator();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.AddNewRadarPositionTextBlock("Consolas", Brushes.Red, 8, new Point<double>());

            string _thrustAniPath = @"..\..\..\Assets\Graphics\Animation\AirThrust32x32.png";
            var playMode = new ActorAnimation.PlayMode()
            {
                Replay = ActorAnimation.ReplayMode.LoopedPlay,
                DeleteActorAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };
            ThrustAnimation = new ActorAnimation(_core, _thrustAniPath, new Size(32, 32), 10, playMode);

            ThrustAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(ThrustAnimation, this);

            var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
            ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
            ThrustAnimation.X = X + pointRight.X;
            ThrustAnimation.Y = Y + pointRight.Y;
        }

        public virtual void BeforeCreate()
        {
        }
        public virtual void AfterCreate()
        {
        }

        public override void RotationChanged()
        {
            PositionChanged();
        }

        public override void PositionChanged()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = X + pointRight.X;
                ThrustAnimation.Y = Y + pointRight.Y;
            }
        }

        public static int GetGenericHP()
        {
            return Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);
        }

        public virtual void ApplyMotion(Point<double> frameAppliedOffset)
        {
            if (X < -Constants.Limits.EnemySceneDistanceLimit
                || X >= _core.Display.VisibleSize.Width + Constants.Limits.EnemySceneDistanceLimit
                || Y < -Constants.Limits.EnemySceneDistanceLimit
                || Y >= _core.Display.VisibleSize.Height + Constants.Limits.EnemySceneDistanceLimit)
            {
                QueueForDelete();
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

                    var offset = Utility.AngleFromPointAtDistance(new Angle<double>(requiredAngle), new Point<double>(200, 200));

                    RadarPositionText.Location = _core.Actors.Player.Location + offset + new Point<double>(25, 25);
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

        public virtual void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            if (SelectedSecondaryWeapon != null && _core.Actors.Player != null)
            {
                SelectedSecondaryWeapon.ApplyIntelligence(frameAppliedOffset, _core.Actors.Player); //Enemy lock-on to Player. :O
            }
        }

        public override void Cleanup()
        {
            if (RadarPositionIndicator != null)
            {
                RadarPositionIndicator.QueueForDelete();
                RadarPositionText.QueueForDelete();
            }
            if (ThrustAnimation != null)
            {
                ThrustAnimation.QueueForDelete();
            }
            base.Cleanup();
        }

        public Dictionary<AIBrainTypes, DniNeuralNetwork> Brains { get; private set; } = new();
    }
}
