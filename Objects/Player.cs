using AI2D.Engine;

namespace AI2D.Objects
{
    public class Player : BaseObject
    {
        #region ~/Ctor

        private const string _imagePath = @"..\..\Assets\Graphics\hf000.png";


        public Player(Game game)
        {
            Initialize(game, _imagePath, null);
        }

        #endregion
    }
}
