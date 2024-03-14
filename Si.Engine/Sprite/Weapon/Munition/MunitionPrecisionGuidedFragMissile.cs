using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionPrecisionGuidedFragMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PrecisionGuidedFragMissile.png";

        public MunitionPrecisionGuidedFragMissile(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom,
             SpriteInteractiveBase lockedTarget = null, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, imagePath, lockedTarget, location, angle)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = SiPoint.DegreesToRadians(8);
        }
    }
}
