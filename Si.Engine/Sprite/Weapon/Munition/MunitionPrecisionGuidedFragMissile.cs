using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionPrecisionGuidedFragMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PrecisionGuidedFragMissile.png";

        public MunitionPrecisionGuidedFragMissile(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom,
             SpriteBase lockedTarget = null, SiPoint xyOffset = null)
            : base(engine, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = SiPoint.DegreesToRadians(8);
        }
    }
}
