using Si.Engine.Sprites._Superclass;
using Si.Engine.Sprites.Weapons._Superclass;
using Si.Engine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprites.Weapons.Munitions
{
    internal class MunitionFragMissile : SeekingMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\FragMissile.png";

        public MunitionFragMissile(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(engine, weapon, firedFrom, imagePath, xyOffset)
        {
            MaxSeekingObservationDistance = 1000;
            MaxSeekingObservationAngleDegrees = 20;
            SeekingRotationRateRadians = SiPoint.DegreesToRadians(4);
        }
    }
}
