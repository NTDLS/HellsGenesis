using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons;

namespace NebulaSiege.Weapons.Munitions
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    internal class _ProjectileMunitionBase : _MunitionBase
    {
        public _ProjectileMunitionBase(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, string imagePath, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
