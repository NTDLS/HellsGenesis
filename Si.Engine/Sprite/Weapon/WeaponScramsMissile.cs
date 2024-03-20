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
        static string Name { get; } = "Guided Scrams Missile";
        private const string soundPath = @"Sounds\Weapons\ScramsMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponScramsMissile(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponScramsMissile(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiPoint location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionGuidedFragMissile(_engine, this, Owner, lockedTarget, location);

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
                        var pointRight = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Add(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Add(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }
                else
                {
                    foreach (var weaponLock in LockedTargets.Where(o => o.LockType == Library.SiConstants.SiWeaponsLockType.Hard))
                    {
                        if (_toggle)
                        {
                            var pointRight = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(10, 10));
                            _engine.Sprites.Munitions.AddLockedOnTo(this, weaponLock.Sprite, pointRight);
                        }
                        else
                        {
                            var pointLeft = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(10, 10));
                            _engine.Sprites.Munitions.AddLockedOnTo(this, weaponLock.Sprite, pointLeft);
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
