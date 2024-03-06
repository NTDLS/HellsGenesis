using Si.Engine.Sprites._Superclass;
using Si.Engine.Sprites.Weapons._Superclass;
using Si.Engine.Sprites.Weapons.Munitions;
using Si.Engine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprites.Weapons
{
    internal class WeaponScattershot : WeaponBase
    {
        static new string Name { get; } = "Scattershot";
        private const string soundPath = @"Sounds\Weapons\Scattershot.wav";
        private const float soundVolumne = 0.2f;

        public WeaponScattershot(EngineCore engine, SpriteShipBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponScattershot(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 1;
            FireDelayMilliseconds = 25;
            Speed = 11.25f;
            AngleVarianceDegrees = 8;
            SpeedVariancePercent = 0.10f;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionScattershot(_engine, this, _owner, xyOffset);
        }
    }
}
