using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.GraphicObjects.Bullets
{
    public class BulletPulseMeson : BaseBullet
    {
        private const string imagePath = @"..\..\Assets\Graphics\Weapon\BulletPulseMeson.png";

        public BulletPulseMeson(Core core, WeaponBase weapon, BaseGraphicObject firedFrom,
             BaseGraphicObject lockedTarget = null, PointD xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
