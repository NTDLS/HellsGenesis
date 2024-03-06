using Si.Engine.Sprites._Superclass;
using Si.Engine.Sprites.Weapons._Superclass;
using Si.Engine.Sprites.Weapons.Munitions;
using Si.Engine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprites.Weapons
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

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionLancer(_engine, this, _owner, xyOffset);
        }
    }
}

