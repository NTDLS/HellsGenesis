using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.GameEngine.Utility;
using Si.Library.Types.Geometry;

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
            Speed = 18.75;
            AngleVarianceDegrees = 0.00;
            SpeedVariancePercent = 0.00;
            RecoilAmount = 0.65;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
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
                    var pointRight = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + SiMath.DegreesToRadians(90), new SiPoint(10, 10));
                    _gameEngine.Sprites.Munitions.Create(this, pointRight);
                }
                else
                {
                    var pointLeft = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - SiMath.DegreesToRadians(90), new SiPoint(10, 10));
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
