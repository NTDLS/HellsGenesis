using HG.Actors.PowerUp.BaseClasses;
using HG.Engine;
using HG.Types.Geometry;
using HG.Utility;
using System.Drawing;
using System.Linq;

namespace HG.Actors.PowerUp
{
    internal class PowerUpSheild : PowerUpBase
    {
        public const int bountyMultiplier = 1;

        private const string _assetPath = @"Graphics\PowerUp\Sheild\";
        private readonly string[] _imagePaths = {
            #region images.
            "1.png",
            "2.png",
            "3.png"
            #endregion
        };

        private readonly int _repairPoints = 25;

        public PowerUpSheild(Core core)
            : base(core)
        {
            int imageIndex = HgRandom.Random.Next(0, 1000) % _imagePaths.Count();
            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
            _repairPoints *= imageIndex + 1;
        }

        public override void ApplyIntelligence(HgPoint displacementVector)
        {
            if (Intersects(_core.Player.Actor))
            {
                _core.Player.Actor.AddShieldHealth(_repairPoints);
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
