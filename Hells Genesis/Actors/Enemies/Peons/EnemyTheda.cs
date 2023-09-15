using HG.Actors.Enemies.BaseClasses;
using HG.Engine;
using HG.Utility;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies.Peons
{
    internal class EnemyTheda : EnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        private const string _assetPath = @"Graphics\Enemy\Theda\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyTheda(EngineCore core)
            : base(core, hullHealth, bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
        }
    }
}
