using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Sprites;
using NebulaSiege.Game.Utility;
using NebulaSiege.Game.Weapons.BaseClasses;
using NebulaSiege.Game.Weapons.Munitions;

namespace NebulaSiege.Game.Weapons
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
                    var pointRight = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new NsPoint(5, 5));
                    _core.Sprites.Munitions.Create(this, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0)
                {
                    var pointLeft = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new NsPoint(5, 5));
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
