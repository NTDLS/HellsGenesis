using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponBlunderbuss : WeaponBase
    {
        static string Name { get; } = "Blunderbuss";
        private const string soundPath = @"Sounds\Weapons\Blunderbuss.wav";
        private const float soundVolumne = 0.4f;

        public WeaponBlunderbuss(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponBlunderbuss(EngineCore engine)
        : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionBlunderbuss(_engine, this, Owner, location);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();

                if (RoundQuantity > 0)
                {
                    for (int i = -15; i < 15; i++) // Create an initial spread so the bullets dont come from the same point.
                    {
                        var location = Owner.Location + (Owner.Orientation + SiMath.RADIANS_90).PointFromAngleAtDistance(new SiVector(i, i));
                        _engine.Sprites.Munitions.Add(this, location);
                    }
                    RoundQuantity--;
                }

                return true;
            }
            return false;
        }
    }
}
