using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponPhotonTorpedo : WeaponBase
    {
        static new string Name { get; } = "Photon Torpedo";
        private const string soundPath = @"Sounds\Weapons\PhotonTorpedo.wav";
        private const float soundVolumne = 0.4f;

        public WeaponPhotonTorpedo(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPhotonTorpedo(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 15;
            FireDelayMilliseconds = 500;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionPhotonTorpedo(_gameEngine, this, _owner, xyOffset);
        }
    }
}

