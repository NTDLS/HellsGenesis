using AI2D.Engine;
using AI2D.Types;
using System.Linq;

namespace AI2D.GraphicObjects
{
    public class ObjDebug : BaseGraphicObject
    {
        private string _assetStarPath = @"..\..\Assets\Graphics\";
        private string[] _assetStarFiles = {
            #region images.
            "Debug (1).png"
            #endregion
        };

        public ObjDebug(Core core)
            : base(core)
        {
            int _explosionImageIndex = Utility.RandomNumber(0, _assetStarFiles.Count());
            LoadResources(_assetStarPath + _assetStarFiles[_explosionImageIndex]);

            X = 0;
            Y = 0;
            Velocity = new VelocityD();

        }
    }
}
