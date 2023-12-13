using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.BaseClasses;
using NebulaSiege.Weapons.Munitions;

namespace NebulaSiege.Weapons
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\DualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponDualVulcanCannon(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 50;
            Speed = 18;
            AngleVarianceDegrees = 0.5;
            SpeedVariancePercent = 0.05;
            RecoilAmount = 0.10;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    var pointRight = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new NsPoint(5, 5));
                    _core.Sprites.Munitions.Create(this, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0)
                {
                    var pointLeft = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new NsPoint(5, 5));
                    _core.Sprites.Munitions.Create(this, pointLeft);
                    RoundQuantity--;
                }

                ApplyRecoil();

                return true;
            }
            return false;
        }

        public override MunitionBase CreateMunition(NsPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionVulcanCannon(_core, this, _owner, xyOffset);
        }
    }
}
