using Si.GameEngine.Engine;
using Si.GameEngine.Sprites;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Weapons.Munitions
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
