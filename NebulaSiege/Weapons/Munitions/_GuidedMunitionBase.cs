using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility.ExtensionMethods;
using NebulaSiege.Weapons;

namespace HellsGenesis.Weapons.Munitions
{
    /// <summary>
    /// Guided munitions need to be locked onto a target before they are fired. They will adjust heading within given parameters to hit the locked target.
    /// </summary>
    internal class _GuidedMunitionBase : _MunitionBase
    {
        public int MaxGuidedObservationAngleDegrees { get; set; } = 90;
        public int GuidedRotationRateInDegrees { get; set; } = 3;
        public _SpriteBase LockedTarget { get; private set; }

        public _GuidedMunitionBase(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, string imagePath,
             _SpriteBase lockedTarget = null, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            LockedTarget = lockedTarget;
        }

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visable)
                {
                    var deltaAngle = DeltaAngle(LockedTarget);
                    if (deltaAngle.IsBetween(-MaxGuidedObservationAngleDegrees, MaxGuidedObservationAngleDegrees))
                    {

                        if (deltaAngle >= 0) //We might as well turn around clock-wise
                        {
                            Velocity.Angle += GuidedRotationRateInDegrees;
                        }
                        else if (deltaAngle < 0) //We might as well turn around counter clock-wise
                        {
                            Velocity.Angle -= GuidedRotationRateInDegrees;
                        }
                    }
                }
            }

            base.ApplyIntelligence(displacementVector);
        }
    }
}
