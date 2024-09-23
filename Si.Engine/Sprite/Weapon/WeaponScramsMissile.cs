using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System.Linq;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponScramsMissile : WeaponBase
    {
        static string Name { get; } = "Scrams Missile";

        private bool _toggle = false;

        public WeaponScramsMissile(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name)
        {
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                var offset = Owner.Orientation.RotatedBy(90.ToRadians().Invert(_toggle)) * new SiVector(10, 10);

                _toggle = !_toggle;

                if (LockedTargets?.Count > 0)
                {
                    foreach (var weaponLock in LockedTargets.Where(o => o.LockType == Library.SiConstants.SiWeaponsLockType.Hard))
                    {
                        _engine.Sprites.Munitions.AddLockedOnTo(this, weaponLock.Sprite, Owner.Location + offset);
                    }
                }
                else
                {
                    _engine.Sprites.Munitions.Add(this, Owner.Location + offset);
                }

                return true;
            }

            return false;
        }
    }
}
