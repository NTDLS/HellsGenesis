using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponBlunderbuss : WeaponBase
    {
        static new string Name { get; } = "Blunderbuss";
        private const string soundPath = @"Sounds\Weapons\Blunderbuss.wav";
        private const float soundVolumne = 0.4f;

        public WeaponBlunderbuss(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponBlunderbuss(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 250;
            Speed = 22.5f;
            AngleVarianceDegrees = 10.0f;
            SpeedVariancePercent = 0.10f;
            RecoilAmount = 0.045f;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                _gameEngine.Rendering.AddScreenShake(2, 100);

                for (int i = -15; i < 15; i++)
                {
                    if (RoundQuantity > 0 || _owner.IsDrone)
                    {
                        var pointRight = SiPoint.PointFromAngleAtDistance360(_owner.Velocity.Angle + SiPoint.DEG_90_RADS, new SiPoint(i, i));
                        _gameEngine.Sprites.Munitions.Create(this, pointRight);
                        RoundQuantity--;
                    }
                }

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
