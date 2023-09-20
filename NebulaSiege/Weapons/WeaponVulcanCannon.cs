using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons.Bullets;

namespace NebulaSiege.Weapons
{
    internal class WeaponVulcanCannon : _WeaponBase
    {
        static new string Name { get; } = "Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\WeaponVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponVulcanCannon(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 100;
            Speed = 20;
            AngleVarianceDegrees = 1.5;
            SpeedVariancePercent = 0.05;
            RecoilAmount = 0.05;
        }

        public override _BulletBase CreateBullet(NsPoint xyOffset, _SpriteBase targetOfLock = null)
        {
            return new BulletVulcanCannon(_core, this, _owner, xyOffset);
        }
    }
}
