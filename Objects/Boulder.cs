using AI2D.Engine;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AI2D.Objects
{
    public class Boulder : BaseObject
    {

        private string[] _imagePaths = { @"..\..\Assets\Graphics\hf001.png", @"..\..\Assets\Graphics\hf001.png" };

        #region ~/Ctor

        public Boulder(Game game)
        {
            int imagePathIndex = Utility.FlipCoin() ? 1 : 0;

            Initialize(game, _imagePaths[imagePathIndex], null);
        }

        #endregion
    }
}
