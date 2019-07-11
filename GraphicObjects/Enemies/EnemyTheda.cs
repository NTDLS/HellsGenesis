using AI2D.Engine;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyTheda : BaseEnemy
    {
        public const int ScoreMultiplier = 1;

        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private readonly string[] _imagePaths = {
            #region images.
            "Theda (1).png",
            "Theda (2).png",
            "Theda (3).png",
            "Theda (4).png",
            "Theda (5).png",
            "Theda (6).png",
            "Theda (7).png",
            "Theda (8).png"
            #endregion
        };

        public EnemyTheda(Core core)
            : base(core, BaseEnemy.GetGenericHP(), ScoreMultiplier)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
        }
    }
}
