using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons.BaseClasses;

namespace NebulaSiege.Weapons.Munitions
{
    internal class MunitionPulseMeson : EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PulseMeson.png";

        public MunitionPulseMeson(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
