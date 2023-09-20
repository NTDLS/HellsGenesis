using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using System.IO;

namespace NebulaSiege.Weapons.Bullets
{
    internal class BulletScattershot : _BulletBase
    {
        private const string _assetPath = @"Graphics\Weapon\BulletScattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public BulletScattershot(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, null, xyOffset)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
