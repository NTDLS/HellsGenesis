using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Engine;
using HG.Types.Geometry;
using HG.Utility;

namespace HG.Actors.Weapons
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\WeaponDualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(Core core, ActorShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponDualVulcanCannon(Core core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            RoundQuantity = 500;
            Damage = 2;
            FireDelayMilliseconds = 150;
            AngleVariancePercent = 0;
            Speed = 18;
            SpeedVariancePercent = 0;
            RecoilAmount = 0.25;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    var pointRight = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new HgPoint(5, 5));
                    _core.Actors.Bullets.Create(this, _owner, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0)
                {
                    var pointLeft = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new HgPoint(5, 5));
                    _core.Actors.Bullets.Create(this, _owner, pointLeft);
                    RoundQuantity--;
                }

                _owner.Velocity.RecoilPercentage += RecoilAmount;

                return true;
            }
            return false;

        }
    }
}
