using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\DualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponDualVulcanCannon(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 50;
            Speed = 13.5f;
            AngleVarianceDegrees = 0.5f;
            SpeedVariancePercent = 0.05f;
        }

        public override MunitionBase CreateMunition(SiPoint location = null, float? angle = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionVulcanCannon(_engine, this, Owner, location, angle);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    var pointRight = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(5, 5));
                    _engine.Sprites.Munitions.Add(this, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0)
                {
                    var pointLeft = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(5, 5));
                    _engine.Sprites.Munitions.Add(this, pointLeft);
                    RoundQuantity--;
                }

                return true;
            }
            return false;
        }
    }
}
