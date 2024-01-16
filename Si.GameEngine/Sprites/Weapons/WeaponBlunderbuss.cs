using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.GameEngine.Utility;
using Si.Library;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponBlunderbuss : WeaponBase
    {
        static new string Name { get; } = "Blunderbuss";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponBlunderbuss(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponBlunderbuss(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

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
                        _gameEngine.Sprites.Munitions.Create(this, pointRight);
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
            return new MunitionBlunderbuss(_gameEngine, this, _owner, xyOffset);
        }
    }
}
