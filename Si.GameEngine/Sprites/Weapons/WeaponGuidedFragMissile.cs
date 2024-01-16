using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.GameEngine.Utility;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponGuidedFragMissile : WeaponBase
    {
        static new string Name { get; } = "Guided Frag Missile";
        private const string soundPath = @"Sounds\Weapons\GuidedFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponGuidedFragMissile(GameEngineCore gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponGuidedFragMissile(GameEngineCore gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 10;
            FireDelayMilliseconds = 1000;
            Speed = 12;
            SpeedVariancePercent = 0.10;

            CanLockOn = true;
            MinLockDistance = 100;
            MaxLockDistance = 600;
            MaxLocks = 2;
            MaxLockOnAngle = 40;
            ExplodesOnImpact = true;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionGuidedFragMissile(_gameEngine, this, _owner, targetOfLock, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (LockedOnObjects == null || LockedOnObjects.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new SiPoint(10, 10));
                        _gameEngine.Sprites.Munitions.Create(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new SiPoint(10, 10));
                        _gameEngine.Sprites.Munitions.Create(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }
                else
                {
                    foreach (var lockedOn in LockedOnObjects)
                    {
                        if (_toggle)
                        {
                            var pointRight = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new SiPoint(10, 10));
                            _gameEngine.Sprites.Munitions.CreateLockedOnTo(this, lockedOn, pointRight);
                        }
                        else
                        {
                            var pointLeft = SiMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new SiPoint(10, 10));
                            _gameEngine.Sprites.Munitions.CreateLockedOnTo(this, lockedOn, pointLeft);
                        }
                        _toggle = !_toggle;
                    }
                }


                return true;
            }
            return false;

        }
    }
}
