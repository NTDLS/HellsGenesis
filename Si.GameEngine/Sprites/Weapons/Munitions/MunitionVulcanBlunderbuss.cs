using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites.Weapons.Munitions
{
    internal class MunitionBlunderbuss : ProjectileMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\Blunderbuss.png";

        public MunitionBlunderbuss(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, SiVector xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
