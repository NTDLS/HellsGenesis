using AI2D.Engine;
using AI2D.Types;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors
{
    public class ActorDebug : ActorBase
    {
        private const string _assetStarPath = @"..\..\..\Assets\Graphics\";
        private readonly string[] _assetStarFiles = {
            #region images.
            "Debug (1).png"
            #endregion
        };

        public ActorDebug(Core core)
            : base(core)
        {
            int _explosionImageIndex = Utility.RandomNumber(0, _assetStarFiles.Count());
            Initialize(_assetStarPath + _assetStarFiles[_explosionImageIndex], new Size(32, 32));

            X = 0;
            Y = 0;
            Velocity = new VelocityD();

        }
    }
}
