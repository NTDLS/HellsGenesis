using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    internal class ProjectileMunitionBase : MunitionBase
    {
        public ProjectileMunitionBase(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiVector xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
