using HG.AI;
using HG.Engine;
using HG.Types;
using HG.Utility.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace HG.Actors.Enemies
{
    internal class EnemyBase : ActorBase
    {
        public IAIController DefaultAIController { get; set; }
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

            RadarDotSize = new HgPoint<int>(4, 4);
            RadarDotColor = Color.FromArgb(200, 100, 100);

            SetHitPoints(hitPoints);
            ScorePoints = HitPoints * scoreMultiplier;

            RadarPositionIndicator = _core.Actors.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.TextBlocks.CreateRadarPosition("Consolas", Brushes.Red, 8, new HgPoint<double>());

            var playMode = new ActorAnimation.PlayMode()
            {
                Replay = ActorAnimation.ReplayMode.LoopedPlay,
                DeleteActorAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };
            ThrustAnimation = new ActorAnimation(_core, @"..\..\..\Assets\Graphics\Animation\AirThrust32x32.png", new Size(32, 32), 10, playMode);

            ThrustAnimation.Reset();
            _core.Actors.Animations.CreateAt(ThrustAnimation, this);

            var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
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
                var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 180, new HgPoint<double>(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = X + pointRight.X;
                ThrustAnimation.Y = Y + pointRight.Y;
            }
        }

        public static int GetGenericHP(Core core)
        {
            return HgRandom.Random.Next(core.Settings.MinEnemyHealth, core.Settings.MaxEnemyHealth);
        }

        public new void ApplyMotion(HgPoint<double> displacementVector)
        {
            if (X < -_core.Settings.EnemySceneDistanceLimit
                || X >= _core.Display.NatrualScreenSize.Width + _core.Settings.EnemySceneDistanceLimit
                || Y < -_core.Settings.EnemySceneDistanceLimit
                || Y >= _core.Display.NatrualScreenSize.Height + _core.Settings.EnemySceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            if (Velocity.AvailableBoost > 0)
            {
                if (Velocity.BoostPercentage < 1.0) //Ramp up the boost until it is at 100%
                {
                    Velocity.BoostPercentage += _core.Settings.EnemyThrustRampUp;
                }
                Velocity.AvailableBoost -= Velocity.MaxBoost * Velocity.BoostPercentage; //Consume boost.

                if (Velocity.AvailableBoost < 0) //Sanity check available boost.
                {
                    Velocity.AvailableBoost = 0;
                }
            }
            else if (Velocity.BoostPercentage > 0) //Ramp down the boost.
            {
                Velocity.BoostPercentage -= _core.Settings.EnemyThrustRampDown;
                if (Velocity.BoostPercentage < 0)
                {
                    Velocity.BoostPercentage = 0;
                }
            }

            var forwardThrust = Velocity.MaxSpeed * Velocity.ThrottlePercentage;

            if (Velocity.BoostPercentage > 0)
            {
                forwardThrust += Velocity.MaxBoost * Velocity.BoostPercentage;
            }

            X += Velocity.Angle.X * forwardThrust - displacementVector.X;
            Y += Velocity.Angle.Y * forwardThrust - displacementVector.Y;

            //base.ApplyMotion(displacementVector);

            if (RadarPositionIndicator != null)
            {
                if (_core.Display.CurrentScaledScreenBounds.IntersectsWith(Bounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_core.Player.Actor));

                    RadarPositionText.Visable = true;
                    RadarPositionIndicator.Visable = true;

                    double requiredAngle = _core.Player.Actor.AngleTo(this);

                    var offset = HgMath.AngleFromPointAtDistance(new HgAngle<double>(requiredAngle), new HgPoint<double>(200, 200));

                    RadarPositionText.Location = _core.Player.Actor.Location + offset + new HgPoint<double>(25, 25);
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

        public virtual void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            if (SelectedSecondaryWeapon != null && _core.Player.Actor != null)
            {
                SelectedSecondaryWeapon.ApplyIntelligence(displacementVector, _core.Player.Actor); //Enemy lock-on to Player. :O
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

        internal void SetDefaultAIController(IAIController value)
        {
            DefaultAIController = value;
        }
    }
}
