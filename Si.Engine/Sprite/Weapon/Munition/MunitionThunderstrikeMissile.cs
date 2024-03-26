using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionThunderstrikeMissile : SeekingMunitionBase
    {
        private const string imagePath = @"Sprites\Weapon\ThunderstrikeMissile.png";

        public MunitionThunderstrikeMissile(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, SiPoint location = null)
            : base(engine, weapon, firedFrom, imagePath, location)
        {
            MaxSeekingObservationDistance = 1000;
            MaxSeekingObservationAngleDegrees = 20;
            SeekingRotationRateRadians = SiPoint.DegreesToRadians(4);
        }
    }
}
