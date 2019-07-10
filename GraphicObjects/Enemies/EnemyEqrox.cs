using AI2D.Engine;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyEqrox : BaseEnemy
    {
        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private readonly string[] _imagePaths = {
            #region images.
            "Eqrox (1).png",
            "Eqrox (2).png",
            "Eqrox (3).png",
            "Eqrox (4).png",
            "Eqrox (5).png",
            "Eqrox (6).png",
            "Eqrox (7).png",
            "Eqrox (8).png"
            #endregion
        };

        public EnemyEqrox(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
        }
    }
}
