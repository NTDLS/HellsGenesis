using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;

namespace NebulaSiege.Weapons.Munitions
{
    internal class MunitionBlunderbuss : _ProjectileMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\Blunderbuss.png";

        public MunitionBlunderbuss(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
