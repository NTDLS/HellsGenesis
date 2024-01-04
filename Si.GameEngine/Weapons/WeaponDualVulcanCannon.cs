using Si.GameEngine.Engine;
using Si.GameEngine.Sprites;
using Si.GameEngine.Utility;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Weapons
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\DualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(EngineCore gameCore, SpriteShipBase owner)
            : base(gameCore, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponDualVulcanCannon(EngineCore gameCore)
            : base(gameCore, Name, soundPath, soundVolumne) => InitializeWeapon();

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

                if (RoundQuantity > 0 || _owner.IsDrone)
                {
                    var pointRight = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new SiPoint(5, 5));
                    _gameCore.Sprites.Munitions.Create(this, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0 || _owner.IsDrone)
                {
                    var pointLeft = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new SiPoint(5, 5));
                    _gameCore.Sprites.Munitions.Create(this, pointLeft);
                    RoundQuantity--;
                }

                ApplyRecoil();

                return true;
            }
            return false;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionVulcanCannon(_gameCore, this, _owner, xyOffset);
        }
    }
}
