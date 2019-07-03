using AI2D.Engine;

namespace AI2D.Objects
{
    public class Enemy : BaseObject
    {

        private string[] _imagePaths = { @"..\..\Assets\Graphics\hf001.png", @"..\..\Assets\Graphics\hf001.png" };

        #region ~/Ctor

        public Enemy(Game game)
        {
            int imagePathIndex = Utility.FlipCoin() ? 1 : 0;

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            Initialize(game, _imagePaths[imagePathIndex], null);
        }

        #endregion
    }
}
