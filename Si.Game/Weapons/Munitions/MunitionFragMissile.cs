using Si.Game.Engine;
using Si.Game.Sprites;
using Si.Game.Weapons.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Drawing;
using System.IO;

namespace Si.Game.Weapons.Munitions
{
    internal class MunitionFragMissile : SeekingMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\FragMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public MunitionFragMissile(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, xyOffset)
        {
            MaxSeekingObservationDistance = 1000;
            MaxSeekingObservationAngleDegrees = 20;
            SeekingRotationRateInDegrees = 4;

            _selectedHitExplosionAnimationIndex = SiRandom.Generator.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_gameCore, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));
        }
    }
}
