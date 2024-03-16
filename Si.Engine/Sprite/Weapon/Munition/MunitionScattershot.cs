using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.IO;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionScattershot : ProjectileMunitionBase
    {
        private const string _assetPath = @"Sprites\Weapon\Scattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public MunitionScattershot(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, null, location, angle)
        {
            selectedImageIndex = SiRandom.Between(0, imageCount - 1);
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
