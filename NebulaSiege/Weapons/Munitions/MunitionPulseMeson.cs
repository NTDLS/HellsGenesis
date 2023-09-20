using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons;

namespace NebulaSiege.Weapons.Munitions
{
    internal class MunitionPulseMeson : _EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PulseMeson.png";

        public MunitionPulseMeson(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
