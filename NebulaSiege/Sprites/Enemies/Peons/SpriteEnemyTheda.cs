using NebulaSiege.Engine;
using NebulaSiege.Utility;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Sprites.Enemies.Peons
{
    internal class SpriteEnemyTheda : _SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        private const string _assetPath = @"Graphics\Enemy\Theda\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public SpriteEnemyTheda(EngineCore core)
            : base(core, hullHealth, bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
        }
    }
}
