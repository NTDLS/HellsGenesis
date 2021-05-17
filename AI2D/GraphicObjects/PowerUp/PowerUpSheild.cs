using AI2D.Engine;
using AI2D.Types;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.PowerUp
{
    public class PowerUpSheild : BasePowerUp
    {
        public const int ScoreMultiplier = 1;

        private const string _assetPath = @"..\..\..\Assets\Graphics\PowerUp\Sheild\";
        private readonly string[] _imagePaths = {
            #region images.
            "1.png",
            "2.png",
            "3.png"
            #endregion
        };

        private int _repairPoints = 25;

        public PowerUpSheild(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();
            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
            _repairPoints *= (imageIndex + 1);
        }

        public new void ApplyIntelligence(PointD frameAppliedOffset)
        {
            if (Intersects(_core.Actors.Player))
            {
                _core.Actors.Player.AddShieldPoints(_repairPoints);
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
