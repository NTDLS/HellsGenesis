using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Weapons
{
    public class WeaponDualVulcanCannon : WeaponBase
    {

        private const string imagePath = @"..\..\Assets\Graphics\Bullet\Vulcan Cannon.png";
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Vulcan Cannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(Core core)
            : base(core, "Dual Vulcan", imagePath, soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 1;
            FireDelayMilliseconds = 150;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _bulletSound.Play();

                if (RoundQuantity > 0)
                {
                    var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + AngleD.Degrees90, new PointD(5, 5));
                    _core.Actors.CreateBullet(_imagePath, Damage, _owner, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0)
                {
                    var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - AngleD.Degrees90, new PointD(5, 5));
                    _core.Actors.CreateBullet(_imagePath, Damage, _owner, pointLeft);
                    RoundQuantity--;
                }

                return true;
            }
            return false;

        }
    }
}
