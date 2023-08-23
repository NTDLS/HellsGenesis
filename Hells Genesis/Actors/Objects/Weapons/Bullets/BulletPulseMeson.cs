using HG.Engine;
using HG.Types;

namespace HG.Actors.Objects.Weapons.Bullets
{
    internal class BulletPulseMeson : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\BulletPulseMeson.png";

        public BulletPulseMeson(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, HGPoint<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
