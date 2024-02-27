using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.GameEngine.Utility;
using Si.Library;
using Si.Library.Types.Geometry;
using System.Drawing;
using System.IO;

namespace Si.GameEngine.Sprites.Weapons.Munitions
{
    internal class MunitionThunderstrikeMissile : SeekingMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\ThunderstrikeMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 66x66\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public MunitionThunderstrikeMissile(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
            MaxSeekingObservationDistance = 1000;
            MaxSeekingObservationAngleDegrees = 20;
            SeekingRotationRateRadians = SiMath.DegreesToRadians(4);

            _selectedHitExplosionAnimationIndex = SiRandom.Between(0, _hitExplosionAnimationCount - 1);
            _hitExplosionAnimation = new SpriteAnimation(_gameEngine, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(66, 66));
        }
    }
}
