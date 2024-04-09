using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionPrecisionGuidedFragMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Sprites\Weapon\PrecisionGuidedFragMissile.png";

        public MunitionPrecisionGuidedFragMissile(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom,
             SpriteInteractiveBase lockedTarget = null, SiVector location = null)
            : base(engine, weapon, firedFrom, imagePath, lockedTarget, location)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = SiMath.DegToRad(8);
        }
    }
}
