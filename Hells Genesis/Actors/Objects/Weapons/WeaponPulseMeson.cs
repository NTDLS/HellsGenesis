using HG.Actors.Objects.Weapons.Bullets;
using HG.Engine;
using HG.Types;

namespace HG.Actors.Objects.Weapons
{
    internal class WeaponPulseMeson : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\WeaponPulseMeson.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPulseMeson(Core core)
            : base(core, "Pulse Meson", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 25;
            FireDelayMilliseconds = 1000;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, HGPoint<double> xyOffset = null)
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
                    var pointRight = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new HGPoint<double>(10, 10));
                    _core.Actors.Bullets.Create(this, _owner, pointRight);
                }
                else
                {
                    var pointLeft = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new HGPoint<double>(10, 10));
                    _core.Actors.Bullets.Create(this, _owner, pointLeft);
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}
