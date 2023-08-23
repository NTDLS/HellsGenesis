using AI2D.Engine;
using System.Drawing;
using System.IO;

namespace AI2D.Actors.Objects.Enemies
{
    internal class EnemyTheda : EnemyBase
    {
        public const int ScoreMultiplier = 1;
        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Theda\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyTheda(Core core)
            : base(core, GetGenericHP(core), ScoreMultiplier)
        {
            selectedImageIndex = Utility.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            AddHitPoints(Utility.Random.Next(_core.Settings.MinEnemyHealth, _core.Settings.MaxEnemyHealth));
        }
    }
}
