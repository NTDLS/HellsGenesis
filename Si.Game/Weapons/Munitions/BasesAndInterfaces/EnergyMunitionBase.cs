using Si.Game.Engine;
using Si.Game.Sprites;
using Si.Game.Weapons.BasesAndInterfaces;
using Si.Shared.Types.Geometry;

namespace Si.Game.Weapons.Munitions
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinkning this one out.
    /// </summary>
    internal class EnergyMunitionBase : MunitionBase
    {
        public EnergyMunitionBase(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
