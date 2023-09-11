using HG.Actors.BaseClasses;
using HG.Actors.PowerUp;
using HG.Actors.PowerUp.BaseClasses;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.AI;
using HG.Engine;
using HG.Types;
using HG.Utility.ExtensionMethods;
using System;
using System.Collections.Generic;

namespace HG.Actors.Enemies.BaseClasses
{
    internal class EnemyBase : ActorShipBase
    {
        public IAIController DefaultAIController { get; set; }
        public Dictionary<Type, IAIController> AIControllers { get; private set; } = new();

        public int CollisionDamage { get; set; } = 25;
        public int ScorePoints { get; private set; } = 25;

        public EnemyBase(Core core, int hitPoints, int scoreMultiplier)
            : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            SetHitPoints(hitPoints);
            ScorePoints = HitPoints * scoreMultiplier;

            RadarPositionIndicator = _core.Actors.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.TextBlocks.CreateRadarPosition(core.DirectX.TextFormats.RadarPositionIndicator, core.DirectX.Colors.Brushes.Red, new HgPoint<double>());
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

        public static int GetGenericHP(Core core)
        {
            return HgRandom.Random.Next(core.Settings.MinEnemyHealth, core.Settings.MaxEnemyHealth);
        }

        public override void Explode()
        {
            _core.Player.Actor.Score += ScorePoints;

            if (HgRandom.ChanceIn(5))
            {
                PowerUpBase powerUp = HgRandom.FlipCoin() ? new PowerUpRepair(_core) : new PowerUpSheild(_core);
                powerUp.Location = Location;
                _core.Actors.Powerups.Insert(powerUp);
            }
            base.Explode();
        }

        public override bool TestHit(HgPoint<double> displacementVector, BulletBase bullet, HgPoint<double> hitTestPosition)
        {
            if (bullet.FiredFromType == HgFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    if (Hit(bullet))
                    {
                        if (HitPoints <= 0)
                        {
                            Explode();
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public override void ApplyMotion(HgPoint<double> displacementVector)
        {
            if (X < -_core.Settings.EnemySceneDistanceLimit
                || X >= _core.Display.NatrualScreenSize.Width + _core.Settings.EnemySceneDistanceLimit
                || Y < -_core.Settings.EnemySceneDistanceLimit
                || Y >= _core.Display.NatrualScreenSize.Height + _core.Settings.EnemySceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            //When an enemy had boost available, it will use it.
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
