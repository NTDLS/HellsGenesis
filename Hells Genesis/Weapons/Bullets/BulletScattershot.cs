using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.Utility;
using System.IO;

namespace HG.Weapons.Bullets
{
    internal class BulletScattershot : _BulletBase
    {
        private const string _assetPath = @"Graphics\Weapon\BulletScattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public BulletScattershot(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom,
             _SpriteBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core, weapon, firedFrom, null, lockedTarget, xyOffset)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}
