using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.Actors.Items.Bullets
{
    internal class BulletVulcanCannon : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\BulletVulcanCannon.png";

        public BulletVulcanCannon(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
