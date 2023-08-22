using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Weapons
{
    public class WeaponDualVulcanCannon : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\WeaponDualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(Core core)
            : base(core, "Dual Vulcan", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 2;
            FireDelayMilliseconds = 150;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new Point<double>(5, 5));
                    _core.Actors.BulletFactory.Create(this, _owner, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0)
                {
                    var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new Point<double>(5, 5));
                    _core.Actors.BulletFactory.Create(this, _owner, pointLeft);
                    RoundQuantity--;
                }

                return true;
            }
            return false;

        }
    }
}
