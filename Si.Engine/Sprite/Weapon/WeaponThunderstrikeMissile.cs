using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponThunderstrikeMissile : WeaponBase
    {
        static string Name { get; } = "Thunderstrike Missile";

        private bool _toggle = false;

        public WeaponThunderstrikeMissile(EngineCore engine, SpriteInteractiveBase owner)
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
                _engine.Sprites.Munitions.Add(this, Owner.Location + offset);

                _toggle = !_toggle;

                return true;
            }

            return false;
        }
    }
}
