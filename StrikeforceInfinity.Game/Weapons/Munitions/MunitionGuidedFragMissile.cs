using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Utility;
using StrikeforceInfinity.Game.Weapons.BasesAndInterfaces;
using System.Drawing;
using System.IO;

namespace StrikeforceInfinity.Game.Weapons.Munitions
{
    internal class MunitionGuidedFragMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\GuidedFragMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public MunitionGuidedFragMissile(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom,
             SpriteBase lockedTarget = null, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = 3;

            _selectedHitExplosionAnimationIndex = HgRandom.Generator.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_gameCore, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));
        }
    }
}
