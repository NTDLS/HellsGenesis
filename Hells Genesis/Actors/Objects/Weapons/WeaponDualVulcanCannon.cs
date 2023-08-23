using HG.Engine;
using HG.Types;

namespace HG.Actors.Objects.Weapons
{
    internal class WeaponDualVulcanCannon : WeaponBase
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
                    var pointRight = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new HGPoint<double>(5, 5));
                    _core.Actors.Bullets.Create(this, _owner, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0)
                {
                    var pointLeft = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new HGPoint<double>(5, 5));
                    _core.Actors.Bullets.Create(this, _owner, pointLeft);
                    RoundQuantity--;
                }

                return true;
            }
            return false;

        }
    }
}
