using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponBlunderbuss : WeaponBase
    {
        static string Name { get; } = "Blunderbuss";

        public WeaponBlunderbuss(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name)
        {
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    for (int i = -15; i < 15; i++) // Create an initial spread so the bullets dont come from the same point.
                    {
                        var offset = Owner.Orientation.RotatedBy(90.ToRadians()) * new SiVector(i, i);
                        _engine.Sprites.Munitions.Add(this, Owner.Location + offset);
                    }
                    RoundQuantity--;
                }

                return true;
            }
            return false;
        }
    }
}
