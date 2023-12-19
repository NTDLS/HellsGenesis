using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Weapons.BaseClasses;
using StrikeforceInfinity.Game.Weapons.Munitions;

namespace StrikeforceInfinity.Game.Weapons
{
    internal class WeaponScattershot : WeaponBase
    {
        static new string Name { get; } = "Scattershot";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.2f;

        public WeaponScattershot(EngineCore gameCore, _SpriteShipBase owner)
            : base(gameCore, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponScattershot(EngineCore gameCore)
            : base(gameCore, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 1;
            FireDelayMilliseconds = 25;
            Speed = 15;
            AngleVarianceDegrees = 8;
            SpeedVariancePercent = 0.10;
            RecoilAmount = 0.01;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionScattershot(_gameCore, this, _owner, xyOffset);
        }
    }
}
