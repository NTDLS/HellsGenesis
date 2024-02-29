using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons.Munitions
{
    internal class MunitionLancer : EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\Lancer.png";

        public MunitionLancer(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, SiVector xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
