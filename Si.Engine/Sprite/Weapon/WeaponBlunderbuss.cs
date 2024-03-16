using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponBlunderbuss : WeaponBase
    {
        static new string Name { get; } = "Blunderbuss";
        private const string soundPath = @"Sounds\Weapons\Blunderbuss.wav";
        private const float soundVolumne = 0.4f;

        public WeaponBlunderbuss(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponBlunderbuss(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 250;
            Speed = 22.5f;
            AngleVarianceDegrees = 10.0f;
            SpeedVariancePercent = 0.10f;
        }

        public override MunitionBase CreateMunition(SiPoint location = null, float? angle = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionBlunderbuss(_engine, this, Owner, location, angle);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    for (int i = -15; i < 15; i++) // Create an initial spread so the bullets dont come from the same point.
                    {
                        var location = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(i, i));
                        _engine.Sprites.Munitions.Create(this, location);
                    }
                    RoundQuantity--;
                }

                return true;
            }
            return false;
        }
    }
}
