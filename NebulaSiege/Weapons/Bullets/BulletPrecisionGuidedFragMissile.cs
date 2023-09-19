using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Weapons.Bullets
{
    internal class BulletPrecisionGuidedFragMissile : _BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\Missiles\BulletPrecisionGuidedFragMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public BulletPrecisionGuidedFragMissile(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom,
             _SpriteBase lockedTarget = null, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            _selectedHitExplosionAnimationIndex = HgRandom.Generator.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));
        }

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.Visable)
                {
                    var deltaAngle = DeltaAngle(LockedTarget);

                    if (deltaAngle >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 8;
                    }
                    else if (deltaAngle < 0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 8;
                    }
                }
            }

            base.ApplyIntelligence(displacementVector);
        }
    }
}
