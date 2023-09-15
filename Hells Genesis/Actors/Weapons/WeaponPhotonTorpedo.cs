using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Engine.Types.Geometry;

namespace HG.Actors.Weapons
{
    internal class WeaponPhotonTorpedo : WeaponBase
    {
        static new string Name { get; } = "Photon Torpedo";
        private const string soundPath = @"Sounds\Weapons\BulletPhotonTorpedo.wav";
        private const float soundVolumne = 0.4f;

        public WeaponPhotonTorpedo(EngineCore core, ActorShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPhotonTorpedo(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 15;
            FireDelayMilliseconds = 500;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, HgPoint xyOffset = null)
        {
            return new BulletPhotonTorpedo(_core, this, _owner, lockedTarget, xyOffset);
        }
    }
}

