using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionPhotonTorpedo : EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PhotonTorpedo.png";

        public MunitionPhotonTorpedo(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(engine, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
