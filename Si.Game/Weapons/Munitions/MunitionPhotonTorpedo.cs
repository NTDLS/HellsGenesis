using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Sprites;
using Si.Game.Weapons.BasesAndInterfaces;

namespace Si.Game.Weapons.Munitions
{
    internal class MunitionPhotonTorpedo : EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PhotonTorpedo.png";

        public MunitionPhotonTorpedo(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
