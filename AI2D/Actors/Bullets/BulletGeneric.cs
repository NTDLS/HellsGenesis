using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.Actors.Bullets
{
    public class BulletGeneric : BulletBase
    {

        public BulletGeneric(Core core, WeaponBase weapon, ActorBase firedFrom, string imagePath,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
