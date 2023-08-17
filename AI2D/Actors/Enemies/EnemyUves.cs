using AI2D.Engine;
using System.Drawing;
using System.IO;

namespace AI2D.Actors.Enemies
{
    public class EnemyUves : EnemyBase
    {
        public const int ScoreMultiplier = 1;
        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Uves\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyUves(Core core)
            : base(core, EnemyBase.GetGenericHP(), ScoreMultiplier)
        {
            selectedImageIndex = Utility.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            base.AddHitPoints(Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth));
        }
    }
}
