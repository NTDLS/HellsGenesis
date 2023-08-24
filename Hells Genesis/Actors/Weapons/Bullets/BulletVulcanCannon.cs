using HG.Engine;
using HG.Types;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletVulcanCannon : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\BulletVulcanCannon.png";

        public BulletVulcanCannon(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
