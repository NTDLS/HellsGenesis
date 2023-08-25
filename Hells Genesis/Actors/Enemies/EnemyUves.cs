using HG.Actors.Enemies.BaseClasses;
using HG.Engine;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies
{
    internal class EnemyUves : EnemyBasicBase
    {
        public const int ScoreMultiplier = 1;
        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Uves\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyUves(Core core)
            : base(core, GetGenericHP(core), ScoreMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            AddHitPoints(HgRandom.Random.Next(_core.Settings.MinEnemyHealth, _core.Settings.MaxEnemyHealth));
        }
    }
}
