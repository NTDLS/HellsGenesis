using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons.Munitions
{
    internal class MunitionScramsMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\ScramsMissile.png";

        public MunitionScramsMissile(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom,
             SpriteBase lockedTarget = null, SiPoint xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = SiPoint.DegreesToRadians(10);
        }
    }
}
