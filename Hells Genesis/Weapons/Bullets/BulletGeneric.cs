using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.BaseClasses;
using HG.Weapons.BaseClasses;
using HG.Weapons.Bullets.BaseClasses;

namespace HG.Weapons.Bullets
{
    internal class BulletGeneric : BulletBase
    {
        public BulletGeneric(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, string imagePath,
             SpriteBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
