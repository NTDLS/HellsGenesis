using HG.Engine;
using HG.Types;
using System.Drawing;
using System.IO;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletFragMissile : BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\Missiles\BulletFragMissile.png";

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        public BulletFragMissile(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {

            _selectedHitExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));
        }

        public override void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            base.ApplyIntelligence(displacementVector);
        }
    }
}
