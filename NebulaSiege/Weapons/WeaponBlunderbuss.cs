using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.BaseClasses;
using NebulaSiege.Weapons.Munitions;

namespace NebulaSiege.Weapons
{
    internal class WeaponBlunderbuss : WeaponBase
    {
        static new string Name { get; } = "Blunderbuss";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponBlunderbuss(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponBlunderbuss(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 250;
            Speed = 30;
            AngleVarianceDegrees = 10.0;
            SpeedVariancePercent = 0.10;
            RecoilAmount = 0.045;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                for (int i = -15; i < 15; i++)
                {
                    if (RoundQuantity > 0)
                    {
                        var pointRight = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new NsPoint(i, i));
                        _core.Sprites.Munitions.Create(this, pointRight);
                        RoundQuantity--;
                    }
                }

                ApplyRecoil();

                return true;
            }
            return false;
        }

        public override MunitionBase CreateMunition(NsPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionBlunderbuss(_core, this, _owner, xyOffset);
        }
    }
}
