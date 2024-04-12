using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponLancer : WeaponBase
    {
        static string Name { get; } = "Lancer";

        public WeaponLancer(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name)
        {
        }
    }
}

