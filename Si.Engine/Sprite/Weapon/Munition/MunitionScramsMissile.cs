using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionScramsMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Sprites\Weapon\ScramsMissile.png";

        public MunitionScramsMissile(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom,
             SpriteInteractiveBase lockedTarget = null, SiVector location = null)
            : base(engine, weapon, firedFrom, imagePath, lockedTarget, location)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = SiVector.DegToRad(10);
        }
    }
}
