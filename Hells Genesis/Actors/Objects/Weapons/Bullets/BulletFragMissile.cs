using HG.Engine;
using HG.Types;

namespace HG.Actors.Objects.Weapons.Bullets
{
    internal class BulletFragMissile : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\Missiles\BulletFragMissile.png";

        public BulletFragMissile(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HGPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }

        public override void ApplyIntelligence(HGPoint<double> frameAppliedOffset, ActorBase testHit)
        {
            base.ApplyIntelligence(frameAppliedOffset, testHit);
        }
    }
}
