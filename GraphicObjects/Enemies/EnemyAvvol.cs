using AI2D.Engine;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyAvvol : BaseEnemy
    {
        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private readonly string[] _imagePaths = {
            #region images.
            "Avvol (1).png",
            "Avvol (2).png",
            "Avvol (3).png",
            "Avvol (4).png",
            "Avvol (5).png",
            "Avvol (6).png",
            "Avvol (7).png",
            "Avvol (8).png"
            #endregion
        };

        public EnemyAvvol(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            LoadResources(_assetPath +_imagePaths[imageIndex], new System.Drawing.Size(32, 32));
        }
    }
}
