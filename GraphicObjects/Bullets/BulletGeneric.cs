using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.GraphicObjects.Bullets
{
    public class BulletGeneric : BaseBullet
    {

        public BulletGeneric(Core core, WeaponBase weapon, BaseGraphicObject firedFrom, string imagePath,
             BaseGraphicObject lockedTarget = null, PointD xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
