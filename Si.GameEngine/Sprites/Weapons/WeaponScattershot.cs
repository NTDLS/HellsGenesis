using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponScattershot : WeaponBase
    {
        static new string Name { get; } = "Scattershot";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.2f;

        public WeaponScattershot(Core.Engine gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponScattershot(Core.Engine gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 1;
            FireDelayMilliseconds = 25;
            Speed = 15;
            AngleVarianceDegrees = 8;
            SpeedVariancePercent = 0.10;
            RecoilAmount = 0.01;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionScattershot(_gameEngine, this, _owner, xyOffset);
        }
    }
}
