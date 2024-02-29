using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\DualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponDualVulcanCannon(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 50;
            Speed = 13.5f;
            AngleVarianceDegrees = 0.5f;
            SpeedVariancePercent = 0.05f;
            RecoilAmount = 0.10f;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0 || _owner.IsDrone)
                {
                    var pointRight = SiVector.PointFromAngleAtDistance360(_owner.Velocity.Angle + SiVector.DEG_90_RADS, new SiVector(5, 5));
                    _gameEngine.Sprites.Munitions.Create(this, pointRight);
                    RoundQuantity--;
                }

                if (RoundQuantity > 0 || _owner.IsDrone)
                {
                    var pointLeft = SiVector.PointFromAngleAtDistance360(_owner.Velocity.Angle - SiVector.DEG_90_RADS, new SiVector(5, 5));
                    _gameEngine.Sprites.Munitions.Create(this, pointLeft);
                    RoundQuantity--;
                }

                ApplyRecoil();

                return true;
            }
            return false;
        }

        public override MunitionBase CreateMunition(SiVector xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionVulcanCannon(_gameEngine, this, _owner, xyOffset);
        }
    }
}
