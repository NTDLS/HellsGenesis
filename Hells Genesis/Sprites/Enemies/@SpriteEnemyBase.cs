using HG.AI;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.PowerUp;
using HG.Utility;
using HG.Utility.ExtensionMethods;
using HG.Weapons.Bullets;
using System;
using System.Collections.Generic;

namespace HG.Sprites.Enemies
{
    internal class SpriteEnemyBase : SpriteShipBase
    {
        public IAIController DefaultAIController { get; set; }
        public Dictionary<Type, IAIController> AIControllers { get; private set; } = new();
        public int CollisionDamage { get; set; } = 25;
        public int BountyWorth { get; private set; } = 25;
        public bool IsHostile { get; set; } = true;

        public SpriteEnemyBase(EngineCore core, int hullHealth, int bountyMultiplier)
            : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            SetHullHealth(hullHealth);
            BountyWorth = HullHealth * bountyMultiplier;

            RadarPositionIndicator = _core.Sprites.RadarPositions.Create();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Sprites.TextBlocks.CreateRadarPosition(
                core.DirectX.TextFormats.RadarPositionIndicator,
                core.DirectX.Materials.Brushes.Red, new HgPoint());
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public override void RotationChanged() => PositionChanged();

        public override void Explode()
        {
            _core.Player.Sprite.Bounty += BountyWorth;

            if (HgRandom.PercentChance(5))
            {
                int random = HgRandom.Between(0, 4);

                SpritePowerUpBase powerUp = null;

                switch (random)
                {
                    case 0:
                        powerUp = new SpritePowerUpAmmo(_core);
                        break;
                    case 1:
                        powerUp = new SpritePowerUpBoost(_core);
                        break;
                    case 2:
                        powerUp = new SpritePowerUpBounty(_core);
                        break;
                    case 3:
                        powerUp = new SpritePowerUpRepair(_core);
                        break;
                    case 4:
                        powerUp = new SpritePowerUpSheild(_core);
                        break;
                }

                if (powerUp != null)
                {
                    powerUp.Location = Location;
                    _core.Sprites.Powerups.Insert(powerUp);
                }
            }
            base.Explode();
        }

        public override bool TestHit(HgPoint displacementVector, BulletBase bullet, HgPoint hitTestPosition)
        {
            if (bullet.FiredFromType == HgFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    if (Hit(bullet))
                    {
                        if (HullHealth <= 0)
                        {
                            Explode();
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public override void ApplyMotion(HgPoint displacementVector)
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

            var thrustVector = Velocity.MaxSpeed * (Velocity.ThrottlePercentage + -Velocity.RecoilPercentage);

            if (Velocity.BoostPercentage > 0)
            {
                thrustVector += Velocity.MaxBoost * Velocity.BoostPercentage;
            }

            X += Velocity.Angle.X * thrustVector - displacementVector.X;
            Y += Velocity.Angle.Y * thrustVector - displacementVector.Y;

            //base.ApplyMotion(displacementVector);

            if (RadarPositionIndicator != null)
            {
                if (_core.Display.GetCurrentScaledScreenBounds().IntersectsWith(Bounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_core.Player.Sprite));

                    RadarPositionText.Visable = true;
                    RadarPositionIndicator.Visable = true;

                    double requiredAngle = _core.Player.Sprite.AngleTo(this);

                    var offset = HgMath.AngleFromPointAtDistance(new HgAngle(requiredAngle), new HgPoint(200, 200));

                    RadarPositionText.Location = _core.Player.Sprite.Location + offset + new HgPoint(25, 25);
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;

                    RadarPositionIndicator.Location = _core.Player.Sprite.Location + offset;
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;
                }
                else
                {
                    RadarPositionText.Visable = false;
                    RadarPositionIndicator.Visable = false;
                }
            }

            if (Velocity.RecoilPercentage > 0)
            {
                Velocity.RecoilPercentage -= Velocity.RecoilPercentage * 0.01;
                if (Velocity.RecoilPercentage < 0.01)
                {
                    Velocity.RecoilPercentage = 0;
                }
            }
        }

        public virtual void ApplyIntelligence(HgPoint displacementVector)
        {
            if (SelectedSecondaryWeapon != null && _core.Player.Sprite != null)
            {
                SelectedSecondaryWeapon.ApplyIntelligence(displacementVector, _core.Player.Sprite); //Enemy lock-on to Player. :O
            }
        }

        internal void AddAIController(IAIController controller)
            => AIControllers.Add(controller.GetType(), controller);

        internal void SetDefaultAIController(IAIController value)
        {
            DefaultAIController = value;
        }
    }
}
