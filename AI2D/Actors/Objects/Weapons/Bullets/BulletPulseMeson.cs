using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Actors.Objects.Weapons.Bullets
{
    internal class BulletPulseMeson : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\BulletPulseMeson.png";

        public BulletPulseMeson(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
