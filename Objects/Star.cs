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
        private string[] _imagePaths = {
            @"..\..\Assets\Graphics\Star1.png",
            @"..\..\Assets\Graphics\Star2.png",
            @"..\..\Assets\Graphics\Star3.png",
            @"..\..\Assets\Graphics\Star4.png"
        };

        public Star(Game game)
            : base(game)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();
            LoadResources(_imagePaths[imageIndex], null);

            X = Utility.Random.Next(0, game.Display.VisibleSize.Width);
            Y = Utility.Random.Next(0, game.Display.VisibleSize.Height);
        }
    }
}
