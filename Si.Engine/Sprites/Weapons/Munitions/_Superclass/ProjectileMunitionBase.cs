using Si.Engine.Sprites._Superclass;
using Si.Engine.Sprites.Weapons._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    internal class ProjectileMunitionBase : MunitionBase
    {
        public ProjectileMunitionBase(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(engine, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
