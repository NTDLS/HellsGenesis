using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Weapons.BaseClasses;

namespace StrikeforceInfinity.Game.Weapons.Munitions
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
