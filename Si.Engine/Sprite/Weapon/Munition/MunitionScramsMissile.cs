using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionScramsMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\ScramsMissile.png";

        public MunitionScramsMissile(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom,
             SpriteInteractiveBase lockedTarget = null, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, imagePath, lockedTarget, location, angle)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = SiPoint.DegreesToRadians(10);
        }
    }
}
