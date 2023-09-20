using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons;

namespace HellsGenesis.Weapons.Projectiles
{
    /// <summary>
    /// Bullet projectiles just go straight.
    /// </summary>
    internal class _BulletProjectileBase : _ProjectileBase
    {
        public _BulletProjectileBase(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, string imagePath, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
        }
    }
}
