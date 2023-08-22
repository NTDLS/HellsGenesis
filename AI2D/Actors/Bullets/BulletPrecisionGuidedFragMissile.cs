using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.Actors.Bullets
{
    internal class BulletPrecisionGuidedFragMissile : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\Missiles\BulletPrecisionGuidedFragMissile.png";

        public BulletPrecisionGuidedFragMissile(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }

        public override void ApplyIntelligence(Point<double> frameAppliedOffset, ActorBase testHit)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visable)
                {
                    var deltaAngle = DeltaAngle(LockedTarget);

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

            base.ApplyIntelligence(frameAppliedOffset, testHit);
        }
    }
}
