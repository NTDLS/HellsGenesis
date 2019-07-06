using AI2D.Engine;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyScinzad : BaseEnemy
    {
        private string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private string[] _imagePaths = {
            #region images.
            "Scinzad (1).png",
            "Scinzad (2).png",
            "Scinzad (3).png",
            "Scinzad (4).png",
            "Scinzad (5).png",
            "Scinzad (6).png",
            "Scinzad (7).png",
            "Scinzad (8).png"
            #endregion
        };

        public EnemyScinzad(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            LoadResources(_assetPath +_imagePaths[imageIndex], new System.Drawing.Size(32, 32));
        }
    }
}
