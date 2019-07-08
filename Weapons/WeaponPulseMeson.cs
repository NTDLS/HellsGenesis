using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Weapons
{
    public class WeaponPulseMeson : WeaponBase
    {

        private const string imagePath = @"..\..\Assets\Graphics\Bullet\Pulse Meson.png";
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Pulse Meson.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPulseMeson(Core core)
            : base(core, "Pulse Meson", imagePath, soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 25;
            FireDelayMilliseconds = 1000;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _bulletSound.Play();
                RoundQuantity--;

                if (_toggle)
                {
                    var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + AngleD.Degrees90, new PointD(10, 10));
                    _core.Actors.CreateBullet(_imagePath, Damage, _owner, pointRight);
                }
                else
                {
                    var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - AngleD.Degrees90, new PointD(10, 10));
                    _core.Actors.CreateBullet(_imagePath, Damage, _owner, pointLeft);
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}
