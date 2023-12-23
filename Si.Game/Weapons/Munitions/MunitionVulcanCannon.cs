using Si.Game.Engine;
using Si.Game.Sprites;
using Si.Game.Weapons.BasesAndInterfaces;
using Si.Shared.Types.Geometry;

namespace Si.Game.Weapons.Munitions
{
    internal class MunitionVulcanCannon : ProjectileMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\VulcanCannon.png";

        public MunitionVulcanCannon(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
