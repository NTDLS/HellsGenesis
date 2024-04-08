using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Guided munitions need to be locked onto a target before they are fired. They will adjust heading within given parameters to hit the locked target.
    /// </summary>
    internal class GuidedMunitionBase : MunitionBase
    {
        public int MaxGuidedObservationAngleDegrees { get; set; } = 90;
        public float GuidedRotationRateInDegrees { get; set; } = SiMath.DegToRad(3);
        public SpriteInteractiveBase LockedTarget { get; private set; }

        public GuidedMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string imagePath,
             SpriteInteractiveBase lockedTarget = null, SiVector location = null)
            : base(engine, weapon, firedFrom, imagePath, location)
        {
            LockedTarget = lockedTarget;
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visable)
                {
                    var deltaAngle = DeltaAngleDegrees(LockedTarget);
                    if (deltaAngle.IsBetween(-MaxGuidedObservationAngleDegrees, MaxGuidedObservationAngleDegrees))
                    {
                        if (deltaAngle >= 0) //We might as well turn around clock-wise
                        {
                            PointingAngle += GuidedRotationRateInDegrees;
                        }
                        else if (deltaAngle < 0) //We might as well turn around counter clock-wise
                        {
                            PointingAngle -= GuidedRotationRateInDegrees;
                        }
                    }
                }
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
