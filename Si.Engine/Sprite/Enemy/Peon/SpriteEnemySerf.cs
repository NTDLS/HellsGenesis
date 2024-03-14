using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Weapon;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemySerf : SpriteEnemyPeonBase
    {
        public SpriteEnemySerf(EngineCore engine)
            : base(engine)
        {
            InitializeSpriteFromMetadata(@"Graphics\Enemy\Peon\Serf\Hull.png");

            Velocity.ForwardAngle.Degrees = AngleTo360(_engine.Player.Sprite);
            _initialHullHealth = HullHealth;
        }

        #region Artificial Intelligence.

        public enum AIMode
        {
            InFormation,
            InFormationTurning,
            Approaching,
            MovingToFallback,
            MovingToApproach,
        }

        private readonly int _initialHullHealth = 0;
        private const float _baseDistanceToKeep = 100;
        private float _distanceToKeep = _baseDistanceToKeep * (SiRandom.NextFloat() + 1f);
        private const float _baseFallbackDistance = 400;
        private float _fallbackDistance;
        private SiAngle fallToAngleRadians;

        public AIMode Mode = AIMode.InFormation;

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            base.ApplyIntelligence(epoch, displacementVector);

            float distanceToPlayer = SiPoint.DistanceTo(this, _engine.Player.Sprite);

            if (Mode == AIMode.InFormation)
            {
                //Since we need to handle the entire "platoon" of formation ships all at once, a good
                //  deal of this AI is handled by the Scenerio engine(s). (see: ScenarioSerfFormations).
                if (distanceToPlayer < 500 && SiRandom.ChanceIn(1, 50000) || HullHealth != _initialHullHealth)
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngleRadians = Velocity.ForwardAngle + (180.0f + SiRandom.Between(0, 10));
                    _fallbackDistance = _baseFallbackDistance * (SiRandom.NextFloat() + 1);
                }
            }

            if (Mode == AIMode.Approaching)
            {
                if (distanceToPlayer > _distanceToKeep)
                {
                    PointAtAndGoto(_engine.Player.Sprite);
                }
                else
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngleRadians = Velocity.ForwardAngle + new SiAngle(180.0f + SiRandom.Between(0, 10)).Radians;
                    _fallbackDistance = _baseFallbackDistance * (SiRandom.NextFloat() + 1);
                }
            }

            if (Mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.ForwardAngle - fallToAngleRadians;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle.Degrees < 0) //We might as well turn around counter clock-wise
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }

                if (distanceToPlayer > _fallbackDistance)
                {
                    Mode = AIMode.MovingToApproach;
                }
            }

            if (Mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngleDegrees(_engine.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 10)
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle < 10)
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }
                else
                {
                    Mode = AIMode.Approaching;
                    _distanceToKeep = _baseDistanceToKeep * (SiRandom.NextFloat() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000)
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            FireWeapon<WeaponDualVulcanCannon>();
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            FireWeapon<WeaponVulcanCannon>();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
