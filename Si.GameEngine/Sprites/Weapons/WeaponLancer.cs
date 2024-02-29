using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponLancer : WeaponBase
    {
        static new string Name { get; } = "Lancer";
        private const string soundPath = @"Sounds\Weapons\Lancer.wav";
        private const float soundVolumne = 0.4f;

        public WeaponLancer(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponLancer(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Speed = 26.25f;
            Damage = 15;
            FireDelayMilliseconds = 100;
            AngleVarianceDegrees = 2.0f;
        }

        public override MunitionBase CreateMunition(SiVector xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionLancer(_gameEngine, this, _owner, xyOffset);
        }
    }
}

