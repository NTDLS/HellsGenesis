using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponPrecisionGuidedFragMissile : WeaponBase
    {
        static string Name { get; } = "Precision Guided Frag Missile";
        private const string soundPath = @"Sounds\Weapons\GuidedFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPrecisionGuidedFragMissile(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponPrecisionGuidedFragMissile(EngineCore engine)
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

                var basePosition = Owner.Location + SiPoint.PointFromAngleAtDistance360(
                    Owner.Direction + SiPoint.RADIANS_90 * (_toggle ? 1 : -1), new SiPoint(10, 10));

                _toggle = !_toggle;

                if (LockedTargets?.Count > 0)
                {
                    foreach (var weaponLock in LockedTargets.Where(o => o.LockType == Library.SiConstants.SiWeaponsLockType.Hard))
                    {
                        _engine.Sprites.Munitions.AddLockedOnTo(this, weaponLock.Sprite, basePosition);
                    }
                }
                else
                {
                    _engine.Sprites.Munitions.Add(this, basePosition);
                }

                return true;
            }
            return false;
        }
    }
}
