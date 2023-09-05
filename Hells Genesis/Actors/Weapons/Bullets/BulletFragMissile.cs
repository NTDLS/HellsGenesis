using HG.Engine;
using HG.Types;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletFragMissile : BulletBase
    {
        private const string imagePath = @"Graphics\Weapon\Missiles\BulletFragMissile.png";

        public BulletFragMissile(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            InitializeGenericBasic(imagePath);
        }

        public override void ApplyIntelligence(HgPoint<double> displacementVector, ActorBase testHit)
        {
            base.ApplyIntelligence(displacementVector, testHit);
        }
    }
}
