using HG.Actors.Objects.Weapons.Bullets;
using HG.Engine;
using HG.Types;

namespace HG.Actors.Objects.Weapons
{
    internal class WeaponPhotonTorpedo : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\BulletPhotonTorpedo.wav";
        private const float soundVolumne = 0.4f;

        public WeaponPhotonTorpedo(Core core)
            : base(core, "Photon Torpedo", soundPath, soundVolumne)
        {
            RoundQuantity = 10;
            Damage = 15;
            FireDelayMilliseconds = 500;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, HGPoint<double> xyOffset = null)
        {
            return new BulletPhotonTorpedo(_core, this, _owner, lockedTarget, xyOffset);
        }
    }
}

