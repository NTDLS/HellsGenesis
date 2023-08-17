using AI2D.Engine;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors.Enemies
{
    public class EnemyTheda : EnemyBase
    {
        public const int ScoreMultiplier = 1;

        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Theda\";
        private readonly string[] _imagePaths = {
            #region images.
            "1.png",
            "2.png",
            "3.png",
            "4.png",
            "5.png",
            "6.png"
            #endregion
        };

        public EnemyTheda(Core core)
            : base(core, EnemyBase.GetGenericHP(), ScoreMultiplier)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            base.AddHitPoints(Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth));

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
        }
    }
}
