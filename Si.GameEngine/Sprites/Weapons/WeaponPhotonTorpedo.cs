using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponPhotonTorpedo : WeaponBase
    {
        static new string Name { get; } = "Photon Torpedo";
        private const string soundPath = @"Sounds\Weapons\PhotonTorpedo.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPhotonTorpedo(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPhotonTorpedo(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 25;
            FireDelayMilliseconds = 1000;
            Speed = 18.75f;
            AngleVarianceDegrees = 0.00f;
            SpeedVariancePercent = 0.00f;
            RecoilAmount = 0.65f;
        }

        public override MunitionBase CreateMunition(SiVector xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionPhotonTorpedo(_gameEngine, this, _owner, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (_toggle)
                {
                    var pointRight = SiVector.PointFromAngleAtDistance360(_owner.Velocity.Angle + SiVector.DEG_90_RADS, new SiVector(10, 10));
                    _gameEngine.Sprites.Munitions.Create(this, pointRight);
                }
                else
                {
                    var pointLeft = SiVector.PointFromAngleAtDistance360(_owner.Velocity.Angle - SiVector.DEG_90_RADS, new SiVector(10, 10));
                    _gameEngine.Sprites.Munitions.Create(this, pointLeft);
                }

                _toggle = !_toggle;

                ApplyRecoil();

                return true;
            }
            return false;

        }
    }
}
