using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinking this one out.
    /// </summary>
    internal class EnergyMunitionBase : MunitionBase
    {
        public EnergyMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string imagePath, SiVector location = null)
            : base(engine, weapon, firedFrom, imagePath, location)
        {
        }
    }
}
