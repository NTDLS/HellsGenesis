using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionVulcanCannon : ProjectileMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\VulcanCannon.png";

        public MunitionVulcanCannon(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, imagePath, location, angle)
        {
            Initialize(imagePath);
        }
    }
}
