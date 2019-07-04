using AI2D.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Objects
{
    public class Star : BaseObject
    {
        private string _assetStarPath = @"..\..\Assets\Graphics\Star\";
        private string[] _assetStarFiles = {
            #region images.
            "Star 1.png",
            "Star 2.png",
            "Star 3.png",
            "Star 4.png",
            #endregion
        };

        public Star(Game game)
            : base(game)
        {
            int _explosionImageIndex = Utility.RandomNumber(0, _assetStarFiles.Count());

            LoadResources(_assetStarPath + _assetStarFiles[_explosionImageIndex]);

            X = Utility.Random.Next(0, game.Display.VisibleSize.Width);
            Y = Utility.Random.Next(0, game.Display.VisibleSize.Height);
        }
    }
}
