using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.IO;

namespace Si.GameEngine.Sprites.Weapons.Munitions
{
    internal class MunitionScattershot : ProjectileMunitionBase
    {
        private const string _assetPath = @"Graphics\Weapon\Scattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public MunitionScattershot(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameEngine, weapon, firedFrom, null, xyOffset)
        {
            selectedImageIndex = SiRandom.Between(0, imageCount - 1);
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
