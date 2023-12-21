using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Weapons.BasesAndInterfaces;

namespace StrikeforceInfinity.Game.Weapons.Munitions
{
    internal class MunitionBlunderbuss : ProjectileMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\Blunderbuss.png";

        public MunitionBlunderbuss(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
