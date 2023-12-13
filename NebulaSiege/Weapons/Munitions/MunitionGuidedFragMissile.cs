using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.BaseClasses;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Weapons.Munitions
{
    internal class MunitionGuidedFragMissile : GuidedMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\GuidedFragMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public MunitionGuidedFragMissile(EngineCore core, WeaponBase weapon, SpriteBase firedFrom,
             SpriteBase lockedTarget = null, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            MaxGuidedObservationAngleDegrees = 90;
            GuidedRotationRateInDegrees = 3;

            _selectedHitExplosionAnimationIndex = HgRandom.Generator.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));
        }
    }
}
