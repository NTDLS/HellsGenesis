using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponPhotonTorpedo : WeaponBase
    {
        static string Name { get; } = "Photon Torpedo";

        private bool _toggle = false;

        public WeaponPhotonTorpedo(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name)
        {
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _engine.Rendering.AddScreenShake(4, 100);
                _fireSound.Play();
                RoundQuantity--;

                if (_toggle)
                {
                    var offsetRight = Owner.Orientation.RotatedBy(90.ToRadians()) * new SiVector(10, 10);
                    _engine.Sprites.Munitions.Add(this, Owner.Location + offsetRight);
                }
                else
                {
                    var offsetLeft = Owner.Orientation.RotatedBy(-90.ToRadians()) * new SiVector(10, 10);
                    _engine.Sprites.Munitions.Add(this, Owner.Location + offsetLeft);
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}
