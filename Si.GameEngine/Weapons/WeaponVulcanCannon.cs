using Si.GameEngine.Engine;
using Si.GameEngine.Sprites;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Weapons
{
    internal class WeaponVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(EngineCore gameCore, SpriteShipBase owner)
            : base(gameCore, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponVulcanCannon(EngineCore gameCore)
            : base(gameCore, Name, soundPath, soundVolumne) => InitializeWeapon();

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
            return new MunitionVulcanCannon(_gameCore, this, _owner, xyOffset);
        }
    }
}
