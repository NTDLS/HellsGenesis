using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.Actors.Bullets
{
    internal class BulletHotPepper : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\BulletHotPepper.png";

        public BulletHotPepper(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
