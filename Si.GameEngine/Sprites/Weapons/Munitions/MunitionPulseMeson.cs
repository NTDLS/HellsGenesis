using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Sprites.Weapons.Munitions
{
    internal class MunitionPulseMeson : EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PulseMeson.png";

        public MunitionPulseMeson(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
