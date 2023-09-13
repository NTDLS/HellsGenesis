using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Types.Geometry;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletGeneric : BulletBase
    {
        public BulletGeneric(Core core, WeaponBase weapon, ActorBase firedFrom, string imagePath,
             ActorBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
