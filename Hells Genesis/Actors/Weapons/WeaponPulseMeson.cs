using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Types.Geometry;
using HG.Utility;

namespace HG.Actors.Weapons
{
    internal class WeaponPulseMeson : WeaponBase
    {
        static new string Name { get; } = "Pulse Meson";
        private const string soundPath = @"Sounds\Weapons\WeaponPulseMeson.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPulseMeson(Core core, ActorShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPulseMeson(Core core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            RoundQuantity = 500;
            Damage = 25;
            FireDelayMilliseconds = 1000;

            RoundQuantity = 500;
            Damage = 25;
            FireDelayMilliseconds = 1000;
            Speed = 25;
            AngleVariancePercent = 0.00;
            SpeedVariancePercent = 0.00;
            RecoilAmount = 0.65;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, HgPoint xyOffset = null)
        {
            return new BulletPulseMeson(_core, this, _owner, lockedTarget, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (_toggle)
                {
                    var pointRight = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new HgPoint(10, 10));
                    _core.Actors.Bullets.Create(this, _owner, pointRight);
                }
                else
                {
                    var pointLeft = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new HgPoint(10, 10));
                    _core.Actors.Bullets.Create(this, _owner, pointLeft);
                }

                _toggle = !_toggle;

                ApplyRecoil();

                return true;
            }
            return false;

        }
    }
}
