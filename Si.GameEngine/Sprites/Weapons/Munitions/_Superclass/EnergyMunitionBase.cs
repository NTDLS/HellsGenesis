using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinkning this one out.
    /// </summary>
    internal class EnergyMunitionBase : MunitionBase
    {
        public EnergyMunitionBase(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiVector xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
