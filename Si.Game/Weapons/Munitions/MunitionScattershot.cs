using Si.Game.Engine;
using Si.Game.Sprites;
using Si.Game.Weapons.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.IO;

namespace Si.Game.Weapons.Munitions
{
    internal class MunitionScattershot : ProjectileMunitionBase
    {
        private const string _assetPath = @"Graphics\Weapon\Scattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public MunitionScattershot(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, null, xyOffset)
        {
            selectedImageIndex = SiRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
