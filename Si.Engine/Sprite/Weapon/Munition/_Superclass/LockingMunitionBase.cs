using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Guided munitions need to be locked onto a target before they are fired. They will adjust heading within given parameters to hit the locked target.
    /// </summary>
    internal class LockingMunitionBase : MunitionBase
    {
        public int MaxGuidedObservationAngleDegrees { get; set; } = 90;
        public float GuidedRotationRateInRadians { get; set; } = SiMath.DegToRad(3);
        public SpriteInteractiveBase? LockedTarget { get; private set; }

        public LockingMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string? imagePath,
             SpriteInteractiveBase? lockedTarget = null, SiVector? location = null)
            : base(engine, weapon, firedFrom, imagePath, location)
        {
            LockedTarget = lockedTarget;
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visible)
                {
                    var deltaAngle = this.HeadingAngleToInSignedDegrees(LockedTarget);

                    if (Math.Abs((float)deltaAngle) < MaxGuidedObservationAngleDegrees && !deltaAngle.IsNearZero())
                    {
                        RotateMovementVector(GuidedRotationRateInRadians * (deltaAngle > 0 ? 1 : -1));
                    }
                }
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
