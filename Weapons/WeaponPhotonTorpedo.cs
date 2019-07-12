using AI2D.Engine;
using AI2D.GraphicObjects;
using AI2D.GraphicObjects.Bullets;
using AI2D.Types;

namespace AI2D.Weapons
{
    public class WeaponPhotonTorpedo : WeaponBase
    {
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Photon Torpedo.wav";
        private const float soundVolumne = 0.4f;

        public WeaponPhotonTorpedo(Core core)
            : base(core, "Photon Torpedo", soundPath, soundVolumne)
        {
            RoundQuantity = 10;
            Damage = 15;
            FireDelayMilliseconds = 500;
        }

        public override BaseBullet CreateBullet(BaseGraphicObject lockedTarget, PointD xyOffset = null)
        {
            return new BulletPhotonTorpedo(_core, this, _owner, lockedTarget, xyOffset);
        }
    }
}

