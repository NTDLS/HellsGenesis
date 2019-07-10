using AI2D.Engine;
using AI2D.GraphicObjects;
using AI2D.GraphicObjects.Bullets;
using AI2D.Types;

namespace AI2D.Weapons
{
    public class WeaponPulseMeson : WeaponBase
    {
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Pulse Meson.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPulseMeson(Core core)
            : base(core, "Pulse Meson", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 25;
            FireDelayMilliseconds = 1000;
        }

        public override BaseBullet CreateBullet(BaseGraphicObject lockedTarget, PointD xyOffset = null)
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
                    var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new PointD(10, 10));
                    _core.Actors.CreateBullet(this, _owner, pointRight);
                }
                else
                {
                    var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new PointD(10, 10));
                    _core.Actors.CreateBullet(this, _owner, pointLeft);
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}
