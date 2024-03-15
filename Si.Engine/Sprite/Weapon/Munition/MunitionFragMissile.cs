using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionFragMissile : SeekingMunitionBase
    {
        private const string imagePath = @"Sprites\Weapon\FragMissile.png";

        public MunitionFragMissile(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, imagePath, location, angle)
        {
            MaxSeekingObservationDistance = 1000;
            MaxSeekingObservationAngleDegrees = 20;
            SeekingRotationRateRadians = SiPoint.DegreesToRadians(4);
        }
    }
}
