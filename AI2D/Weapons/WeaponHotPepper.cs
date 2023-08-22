using AI2D.Actors;
using AI2D.Actors.Bullets;
using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Weapons
{
    internal class WeaponHotPepper : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\WeaponVulcanCannon.wav";
        private const float soundVolumne = 0.2f;

        public WeaponHotPepper(Core core)
            : base(core, "Hot Pepper", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 1;
            FireDelayMilliseconds = 25;
            AngleSlop = 4;
            Speed = 11;
            SpeedSlop = 10;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, Point<double> xyOffset = null)
        {
            return new BulletHotPepper(_core, this, _owner, lockedTarget, xyOffset);
        }
    }
}
