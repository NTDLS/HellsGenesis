using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.GameEngine.Utility;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons
{
    internal class WeaponPrecisionGuidedFragMissile : WeaponBase
    {
        static new string Name { get; } = "Precision Guided Frag Missile";
        private const string soundPath = @"Sounds\Weapons\GuidedFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPrecisionGuidedFragMissile(Core.Engine gameEngine, SpriteShipBase owner)
            : base(gameEngine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPrecisionGuidedFragMissile(Core.Engine gameEngine)
            : base(gameEngine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 8;
            FireDelayMilliseconds = 800;
            Speed = 13;
            SpeedVariancePercent = 0.10;

            CanLockOn = true;
            MinLockDistance = 100;
            MaxLockDistance = 1000;
            MaxLocks = 1;
            MaxLockOnAngle = 45;
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
