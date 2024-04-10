using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponThunderstrikeMissile : WeaponBase
    {
        static string Name { get; } = "Thunderstrike Missile";
        private const string soundPath = @"Sounds\Weapons\ThunderstrikeMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponThunderstrikeMissile(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponThunderstrikeMissile(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionFragMissile(_engine, this, Owner, location);
        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                var offset = Owner.Orientation.RotatedBy(90.ToRadians().Invert(_toggle)) * new SiVector(10, 10);
                _toggle = !_toggle;

                _engine.Sprites.Munitions.Add(this, Owner.Location + offset);

                return true;
            }
            return false;

        }
    }
}
