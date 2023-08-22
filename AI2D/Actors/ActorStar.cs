using AI2D.Engine;
using System.Linq;

namespace AI2D.Actors
{
    internal class ActorStar : ActorBase
    {
        private const string _assetStarPath = @"..\..\..\Assets\Graphics\Star\";
        private readonly string[] _assetStarFiles = {
            #region images.
            "1.png",
            "2.png",
            "3.png",
            "4.png",
            #endregion
        };

        public ActorStar(Core core)
            : base(core)
        {
            int _ImageIndex = Utility.RandomNumber(0, _assetStarFiles.Count());
            Initialize(_assetStarPath + _assetStarFiles[_ImageIndex]);

            X = Utility.Random.Next(0, core.Display.TotalCanvasSize.Width);
            Y = Utility.Random.Next(0, core.Display.TotalCanvasSize.Height);

            //Velocity.ThrottlePercentage = 1;

            if (_ImageIndex == 0 || _ImageIndex == 1)
            {
                Velocity.ThrottlePercentage = (Utility.Random.Next(8, 10) / 10.0);
            }
            else
            {
                Velocity.ThrottlePercentage = (Utility.Random.Next(4, 8) / 10.0);
            }

        }
    }
}
