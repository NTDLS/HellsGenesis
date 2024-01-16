using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;
using System.Drawing;
using System.IO;

namespace Si.GameEngine.Sprites.Weapons.Munitions
{
    internal class MunitionScramsMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\ScramsMissile.png";
        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public MunitionScramsMissile(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom,
             SpriteBase lockedTarget = null, SiPoint xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = 10;

            _selectedHitExplosionAnimationIndex = SiRandom.Generator.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_gameEngine, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));

        }
    }
}
