using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponScattershot : WeaponBase
    {
        static string Name { get; } = "Scattershot";

        public WeaponScattershot(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name)
        {
        }
    }
}
