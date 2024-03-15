using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionPhotonTorpedo : EnergyMunitionBase
    {
        private const string imagePath = @"Sprites\Weapon\PhotonTorpedo.png";

        public MunitionPhotonTorpedo(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, imagePath, location, angle)
        {
            Initialize(imagePath);
        }
    }
}
