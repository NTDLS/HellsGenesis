using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponLancer : WeaponBase
    {
        static string Name { get; } = "Lancer";
        private const string soundPath = @"Sounds\Weapons\Lancer.wav";
        private const float soundVolumne = 0.4f;

        public WeaponLancer(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponLancer(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiPoint location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionLancer(_engine, this, Owner, location);
    }
}

