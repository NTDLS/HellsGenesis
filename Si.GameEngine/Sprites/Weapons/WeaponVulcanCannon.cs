using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponVulcanCannon(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 25;
            Speed = 20;
            AngleVarianceDegrees = 1.5;
            SpeedVariancePercent = 0.05;
            RecoilAmount = 0.05;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionVulcanCannon(_gameEngine, this, _owner, xyOffset);
        }
    }
}
