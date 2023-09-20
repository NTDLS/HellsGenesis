using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons;

namespace HellsGenesis.Weapons.Projectiles
{
    internal class ProjectilePhotonTorpedo : _EnergyProjectileBase
    {
        private const string imagePath = @"Graphics\Weapon\PhotonTorpedo.png";

        public ProjectilePhotonTorpedo(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
