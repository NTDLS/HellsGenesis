using AI2D.Actors;
using AI2D.Actors.Bullets;
using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Weapons
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

        public override BulletBase CreateBullet(ActorBase lockedTarget, Point<double> xyOffset = null)
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
                    var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new Point<double>(10, 10));
                    _core.Actors.BulletFactory.Create(this, _owner, pointRight);
                }
                else
                {
                    var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new Point<double>(10, 10));
                    _core.Actors.BulletFactory.Create(this, _owner, pointLeft);
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}
