using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.BaseClasses;
using HG.Weapons.BaseClasses;
using HG.Weapons.Bullets.BaseClasses;

namespace HG.Weapons.Bullets
{
    internal class BulletVulcanCannon : BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\BulletVulcanCannon.png";

        public BulletVulcanCannon(EngineCore core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
