using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons.Bullets;

namespace NebulaSiege.Weapons
{
    internal class WeaponPhotonTorpedo : _WeaponBase
    {
        static new string Name { get; } = "Photon Torpedo";
        private const string soundPath = @"Sounds\Weapons\BulletPhotonTorpedo.wav";
        private const float soundVolumne = 0.4f;

        public WeaponPhotonTorpedo(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPhotonTorpedo(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 15;
            FireDelayMilliseconds = 500;
        }

        public override _BulletBase CreateBullet(NsPoint xyOffset, _SpriteBase targetOfLock = null)
        {
            return new BulletPhotonTorpedo(_core, this, _owner,  xyOffset);
        }
    }
}

