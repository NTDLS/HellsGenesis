using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.BaseClasses;
using HG.Weapons.BaseClasses;
using HG.Weapons.Bullets.BaseClasses;

namespace HG.Weapons.Bullets
{
    internal class BulletPhotonTorpedo : BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\BulletPhotonTorpedo.png";

        public BulletPhotonTorpedo(EngineCore core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
