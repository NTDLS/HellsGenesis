using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponFragMissile : WeaponBase
    {
        static string Name { get; } = "Frag Missile";
        private const string soundPath = @"Sounds\Weapons\FragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponFragMissile(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) { }

        public WeaponFragMissile(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) { }

        public override MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionFragMissile(_engine, this, Owner, location);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                var basePosition = Owner.Location
                    + (Owner.Orientation + SiMath.RADIANS_90 * (_toggle ? 1 : -1)).PointFromAngleAtDistance(new SiVector(10, 10));

                _engine.Sprites.Munitions.Add(this, basePosition);

                _toggle = !_toggle;

                return true;
            }

            return false;
        }
    }
}
