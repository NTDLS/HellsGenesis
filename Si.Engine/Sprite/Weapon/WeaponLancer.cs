using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponLancer : WeaponBase
    {
        static new string Name { get; } = "Lancer";
        private const string soundPath = @"Sounds\Weapons\Lancer.wav";
        private const float soundVolumne = 0.4f;

        public WeaponLancer(EngineCore engine, SpriteShipBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponLancer(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Speed = 26.25f;
            Damage = 15;
            FireDelayMilliseconds = 100;
            AngleVarianceDegrees = 2.0f;
        }

        public override MunitionBase CreateMunition(SiPoint location = null, float? angle = null, SpriteBase lockedTarget = null)
            => new MunitionLancer(_engine, this, Owner, location, angle);
    }
}

