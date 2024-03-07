using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponScramsMissile : WeaponBase
    {
        static new string Name { get; } = "Guided Scrams Missile";
        private const string soundPath = @"Sounds\Weapons\ScramsMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponScramsMissile(EngineCore engine, SpriteShipBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponScramsMissile(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 5;
            FireDelayMilliseconds = 500;
            Speed = 8.25f;
            SpeedVariancePercent = 0.10f;

            CanLockOn = true;
            MinLockDistance = 50;
            MaxLockDistance = 1500;
            MaxLocks = 8;
            MaxLockOnAngle = 50;
            ExplodesOnImpact = true;
        }

        public override MunitionBase CreateMunition(SiPoint location = null, float? angle = null, SpriteBase lockedTarget = null)
            => new MunitionGuidedFragMissile(_engine, this, Owner, lockedTarget, location, angle);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (LockedTargets == null || LockedTargets.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.Angle + SiPoint.DEG_90_RADS, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Create(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.Angle - SiPoint.DEG_90_RADS, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Create(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }
                else
                {
                    foreach (var weaponLock in LockedTargets.Where(o => o.LockType == Library.SiConstants.SiWeaponsLockType.Hard))
                    {
                        if (_toggle)
                        {
                            var pointRight = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.Angle + SiPoint.DEG_90_RADS, new SiPoint(10, 10));
                            _engine.Sprites.Munitions.CreateLockedOnTo(this, weaponLock.Sprite, pointRight);
                        }
                        else
                        {
                            var pointLeft = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.Angle - SiPoint.DEG_90_RADS, new SiPoint(10, 10));
                            _engine.Sprites.Munitions.CreateLockedOnTo(this, weaponLock.Sprite, pointLeft);
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
