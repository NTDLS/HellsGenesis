using AI2D.Engine;
using System.Linq;

namespace AI2D.Objects
{
    public class ObjEnemy : ObjBase
    {
        private string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private string[] _imagePaths = {
            #region images.
            "xenis-blue-a-1.png",
            "xenis-blue-a-2.png",
            "xenis-blue-a-3.png",
            "xenis-blue-b-1.png",
            "xenis-blue-b-2.png",
            "xenis-blue-b-3.png",
            "xenis-blue-c-1.png",
            "xenis-blue-c-2.png",
            "xenis-blue-c-3.png",
            "xenis-green-a-1.png",
            "xenis-green-a-2.png",
            "xenis-green-a-3.png",
            "xenis-green-b-1.png",
            "xenis-green-b-2.png",
            "xenis-green-b-3.png",
            "xenis-green-c-1.png",
            "xenis-green-c-2.png",
            "xenis-green-c-3.png",
            "xenis-orange-a-1.png",
            "xenis-orange-a-2.png",
            "xenis-orange-a-3.png",
            "xenis-orange-b-1.png",
            "xenis-orange-b-2.png",
            "xenis-orange-b-3.png",
            "xenis-orange-c-1.png",
            "xenis-orange-c-2.png",
            "xenis-orange-c-3.png",
            "xenis-purple-a-1.png",
            "xenis-purple-a-2.png",
            "xenis-purple-a-3.png",
            "xenis-purple-b-1.png",
            "xenis-purple-b-2.png",
            "xenis-purple-b-3.png",
            "xenis-purple-c-1.png",
            "xenis-purple-c-2.png",
            "xenis-purple-c-3.png",
            "xenis-red-1.png",
            "xenis-red-a-2.png",
            "xenis-red-a-3.png",
            "xenis-red-b-1.png",
            "xenis-red-b-2.png",
            "xenis-red-b-3.png",
            "xenis-red-c-1.png",
            "xenis-red-c-2.png",
            "xenis-red-c-3.png",
            "xenis-yellow-a-1.png",
            "xenis-yellow-a-2.png",
            "xenis-yellow-a-3.png",
            "xenis-yellow-b-1.png",
            "xenis-yellow-b-2.png",
            "xenis-yellow-b-3.png",
            "xenis-yellow-c-1.png",
            "xenis-yellow-c-2.png",
            "xenis-yellow-c-3.png"

            #endregion
        };

        public ObjEnemy(Game game)
            : base(game)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            LoadResources(_assetPath +_imagePaths[imageIndex], new System.Drawing.Size(32, 32));
        }
    }
}
