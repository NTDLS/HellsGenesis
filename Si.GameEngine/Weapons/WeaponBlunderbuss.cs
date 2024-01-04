using Si.GameEngine.Engine;
using Si.GameEngine.Sprites;
using Si.GameEngine.Utility;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Weapons
{
    internal class WeaponBlunderbuss : WeaponBase
    {
        static new string Name { get; } = "Blunderbuss";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponBlunderbuss(EngineCore gameCore, SpriteShipBase owner)
            : base(gameCore, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponBlunderbuss(EngineCore gameCore)
            : base(gameCore, Name, soundPath, soundVolumne) => InitializeWeapon();

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
                    if (RoundQuantity > 0 || _owner.IsDrone)
                    {
                        var pointRight = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new SiPoint(i, i));
                        _gameCore.Sprites.Munitions.Create(this, pointRight);
                        RoundQuantity--;
                    }
                }

                ApplyRecoil();

                return true;
            }
            return false;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionBlunderbuss(_gameCore, this, _owner, xyOffset);
        }
    }
}
