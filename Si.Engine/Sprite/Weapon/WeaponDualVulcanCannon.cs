using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        static string Name { get; } = "Dual Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\DualVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponDualVulcanCannon(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionVulcanCannon(_engine, this, Owner, location);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    var offsetRight = Owner.Orientation.RotatedBy(90.ToRadians()) * new SiVector(5, 5);
                    _engine.Sprites.Munitions.Add(this, Owner.Location + offsetRight);

                    var offsetLeft = Owner.Orientation.RotatedBy(-90.ToRadians()) * new SiVector(5, 5);
                    _engine.Sprites.Munitions.Add(this, Owner.Location + offsetLeft);
                }

                RoundQuantity--;

                return true;
            }
            return false;
        }
    }
}
