using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Sprites;
using NebulaSiege.Game.Weapons.BaseClasses;

namespace NebulaSiege.Game.Weapons.Munitions
{
    internal class MunitionPhotonTorpedo : EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PhotonTorpedo.png";

        public MunitionPhotonTorpedo(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
