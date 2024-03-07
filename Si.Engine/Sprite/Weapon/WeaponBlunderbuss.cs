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

        public WeaponBlunderbuss(EngineCore engine, SpriteShipBase owner)
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

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                for (int i = -15; i < 15; i++)
                {
                    if (RoundQuantity > 0)
                    {
                        var pointRight = SiPoint.PointFromAngleAtDistance360(_owner.Velocity.Angle + SiPoint.DEG_90_RADS, new SiPoint(i, i));
                        _engine.Sprites.Munitions.Create(this, pointRight);
                        RoundQuantity--;
                    }
                }

                return true;
            }
            return false;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionBlunderbuss(_engine, this, _owner, xyOffset);
        }
    }
}
