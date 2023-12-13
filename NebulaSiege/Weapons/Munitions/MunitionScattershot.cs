using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.BaseClasses;
using System.IO;

namespace NebulaSiege.Weapons.Munitions
{
    internal class MunitionScattershot : ProjectileMunitionBase
    {
        private const string _assetPath = @"Graphics\Weapon\Scattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public MunitionScattershot(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, null, xyOffset)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
