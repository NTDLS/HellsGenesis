using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Engine.Types.Geometry;

namespace HG.Actors.Weapons.Bullets
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
