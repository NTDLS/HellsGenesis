using HG.Engine;
using HG.Types;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletGeneric : BulletBase
    {

        public BulletGeneric(Core core, WeaponBase weapon, ActorBase firedFrom, string imagePath,
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            InitializeGenericBasic(imagePath);
        }
    }
}
