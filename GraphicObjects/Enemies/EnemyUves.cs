using AI2D.Engine;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyUves : BaseEnemy
    {
        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private readonly string[] _imagePaths = {
            #region images.
            "Uves (1).png",
            "Uves (2).png",
            "Uves (3).png",
            "Uves (4).png",
            "Uves (5).png",
            "Uves (6).png",
            "Uves (7).png",
            "Uves (8).png"
            #endregion
        };

        public EnemyUves(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            LoadResources(_assetPath +_imagePaths[imageIndex], new System.Drawing.Size(32, 32));
        }
    }
}
