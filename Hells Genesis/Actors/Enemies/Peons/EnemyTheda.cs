using HG.Actors.Enemies.BaseClasses;
using HG.Engine;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies.Peons
{
    internal class EnemyTheda : EnemyPeonBase
    {
        public const int ScoreMultiplier = 1;
        private const string _assetPath = @"Graphics\Enemy\Theda\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyTheda(Core core)
            : base(core, GetGenericHP(core), ScoreMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            AddHitPoints(HgRandom.Random.Next(_core.Settings.MinEnemyHealth, _core.Settings.MaxEnemyHealth));
        }
    }
}
