using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;

namespace NebulaSiege.Weapons.Bullets
{
    internal class BulletVulcanCannon : _BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\BulletVulcanCannon.png";

        public BulletVulcanCannon(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
