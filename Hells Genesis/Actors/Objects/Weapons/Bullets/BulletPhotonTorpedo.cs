using HG.Engine;
using HG.Types;

namespace HG.Actors.Objects.Weapons.Bullets
{
    internal class BulletPhotonTorpedo : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\BulletPhotonTorpedo.png";

        public BulletPhotonTorpedo(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HGPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
