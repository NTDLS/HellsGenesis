using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.Utility;
using System.Drawing;
using System.IO;

namespace HG.Weapons.Bullets
{
    internal class BulletPrecisionGuidedFragMissile : BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\Missiles\BulletPrecisionGuidedFragMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public BulletPrecisionGuidedFragMissile(EngineCore core, WeaponBase weapon, SpriteBase firedFrom,
             SpriteBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            _selectedHitExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));
        }

        public override void ApplyIntelligence(HgPoint displacementVector)
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
