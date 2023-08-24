using Hells_Genesis.ExtensionMethods;
using HG.AI;
using HG.Engine;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HG.Actors.Objects.Enemies
{
    internal class EnemyBase : ActorBase
    {
        public IAIController CurrentAIController { get; set; }
        public Dictionary<Type, IAIController> AIControllers { get; private set; } = new();
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

            RadarDotSize = new HGPoint<int>(4, 4);
            RadarDotColor = Color.FromArgb(200, 100, 100);

            SetHitPoints(hitPoints);
            ScorePoints = HitPoints * scoreMultiplier;

            RadarPositionIndicator = _core.Actors.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.TextBlocks.CreateRadarPosition("Consolas", Brushes.Red, 8, new HGPoint<double>());

            var playMode = new ActorAnimation.PlayMode()
            {
                Replay = ActorAnimation.ReplayMode.LoopedPlay,
                DeleteActorAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };
            ThrustAnimation = new ActorAnimation(_core, @"..\..\..\Assets\Graphics\Animation\AirThrust32x32.png", new Size(32, 32), 10, playMode);

            ThrustAnimation.Reset();
            _core.Actors.Animations.CreateAt(ThrustAnimation, this);

            var pointRight = HGMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HGPoint<double>(20, 20));
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
                var pointRight = HGMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HGPoint<double>(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = X + pointRight.X;
                ThrustAnimation.Y = Y + pointRight.Y;
            }
        }

        public static int GetGenericHP(Core core)
        {
            return HGRandom.Random.Next(core.Settings.MinEnemyHealth, core.Settings.MaxEnemyHealth);
        }

        public new void ApplyMotion(HGPoint<double> appliedOffset)
        {
            if (X < -_core.Settings.EnemySceneDistanceLimit
                || X >= _core.Display.NatrualScreenSize.Width + _core.Settings.EnemySceneDistanceLimit
                || Y < -_core.Settings.EnemySceneDistanceLimit
                || Y >= _core.Display.NatrualScreenSize.Height + _core.Settings.EnemySceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            base.ApplyMotion(appliedOffset);

            if (RadarPositionIndicator != null)
            {
                if (_core.Display.CurrentScaledScreenBounds.IntersectsWith(this.Bounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_core.Player.Actor));

                    RadarPositionText.Visable = true;
                    RadarPositionIndicator.Visable = true;

                    double requiredAngle = _core.Player.Actor.AngleTo(this);

                    var offset = HGMath.AngleFromPointAtDistance(new HGAngle<double>(requiredAngle), new HGPoint<double>(200, 200));

                    RadarPositionText.Location = _core.Player.Actor.Location + offset + new HGPoint<double>(25, 25);
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;

                    RadarPositionIndicator.Location = _core.Player.Actor.Location + offset;
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;
                }
                else
                {
                    RadarPositionText.Visable = false;
                    RadarPositionIndicator.Visable = false;
                }
            }
        }

        public virtual void ApplyIntelligence(HGPoint<double> appliedOffset)
        {
            if (SelectedSecondaryWeapon != null && _core.Player.Actor != null)
            {
                SelectedSecondaryWeapon.ApplyIntelligence(appliedOffset, _core.Player.Actor); //Enemy lock-on to Player. :O
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

        internal void AddAIController(IAIController controller)
        {
            AIControllers.Add(controller.GetType(), controller);
        }

        internal void SetCurrentAIController(IAIController value)
        {
            CurrentAIController = value;
        }
    }
}
