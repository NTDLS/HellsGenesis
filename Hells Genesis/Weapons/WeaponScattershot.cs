using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.Weapons.Bullets;

namespace HG.Weapons
{
    internal class WeaponScattershot : _WeaponBase
    {
        static new string Name { get; } = "Scattershot";
        private const string soundPath = @"Sounds\Weapons\WeaponVulcanCannon.wav";
        private const float soundVolumne = 0.2f;

        public WeaponScattershot(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponScattershot(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 1;
            FireDelayMilliseconds = 25;
            Speed = 15;
            AngleVarianceDegrees = 8;
            SpeedVariancePercent = 0.10;
            RecoilAmount = 0.25;
        }

        public override _BulletBase CreateBullet(_SpriteBase lockedTarget, HgPoint xyOffset = null)
        {
            return new BulletScattershot(_core, this, _owner, lockedTarget, xyOffset);
        }
    }
}
