using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponVulcanCannon : WeaponBase
    {
        static string Name { get; } = "Vulcan Cannon";

        public WeaponVulcanCannon(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name)
        {
        }
    }
}
