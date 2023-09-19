using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;

namespace HG.Weapons.Bullets
{
    internal class BulletGeneric : _BulletBase
    {
        public BulletGeneric(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, string imagePath,
             _SpriteBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
