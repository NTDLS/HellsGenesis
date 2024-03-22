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

        public MunitionScattershot(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, SiPoint location = null)
            : base(engine, weapon, firedFrom, null, location)
        {
            SetImage(Path.Combine(_assetPath, $"{SiRandom.Between(0, 3)}.png"));
        }
    }
}
