using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.BaseClasses;
using HG.Utility;
using HG.Weapons.BaseClasses;

namespace HG.Weapons
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\WeaponDualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(EngineCore core, ActorShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponDualVulcanCannon(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 150;
            Speed = 18;
            AngleVarianceDegrees = 0.5;
            SpeedVariancePercent = 0.05;
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

                ApplyRecoil();

                return true;
            }
            return false;

        }
    }
}
