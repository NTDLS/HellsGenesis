using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons;

namespace HellsGenesis.Weapons.Projectiles
{
    /// <summary>
    /// Energy projectiles just go straight - for now.... still thinkning this one out.
    /// </summary>
    internal class _EnergyProjectileBase : _ProjectileBase
    {
        public _EnergyProjectileBase(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, string imagePath, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
