using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons.BaseClasses;

namespace NebulaSiege.Weapons.Munitions
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    internal class ProjectileMunitionBase : MunitionBase
    {
        public ProjectileMunitionBase(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, string imagePath, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
