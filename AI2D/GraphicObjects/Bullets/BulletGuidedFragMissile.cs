using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.GraphicObjects.Bullets
{
    public class BulletGuidedFragMissile : BaseBullet
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\Missiles\BulletGuidedFragMissile.png";

        public BulletGuidedFragMissile(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, PointD xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }

        public override void ApplyIntelligence(PointD frameAppliedOffset, ActorBase testHit)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visable)
                {
                    var deltaAngle = DeltaAngle(LockedTarget);

                    if (deltaAngle >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 0.5;
                    }
                    else if (deltaAngle < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 0.5;
                    }
                }
            }

            base.ApplyIntelligence(frameAppliedOffset, testHit);
        }
    }
}
