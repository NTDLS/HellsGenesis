using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinkning this one out.
    /// </summary>
    internal class EnergyMunitionBase : MunitionBase
    {
        public EnergyMunitionBase(Engine gameCore, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
