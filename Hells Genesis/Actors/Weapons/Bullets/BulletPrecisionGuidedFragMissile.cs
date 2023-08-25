using HG.Engine;
using HG.Types;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletPrecisionGuidedFragMissile : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\Missiles\BulletPrecisionGuidedFragMissile.png";

        public BulletPrecisionGuidedFragMissile(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }

        public override void ApplyIntelligence(HgPoint<double> displacementVector, ActorBase testHit)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visable)
                {
                    var deltaAngle = DeltaAngle360(LockedTarget);

                    if (deltaAngle >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 8;
                    }
                    else if (deltaAngle < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 8;
                    }
                }
            }

            base.ApplyIntelligence(displacementVector, testHit);
        }
    }
}
