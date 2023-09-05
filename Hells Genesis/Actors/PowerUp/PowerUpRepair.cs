using HG.Actors.PowerUp.BaseClasses;
using HG.Engine;
using HG.Types;
using System.Drawing;
using System.Linq;

namespace HG.Actors.PowerUp
{
    internal class PowerUpRepair : PowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Repair\";
        private readonly string[] _imagePaths = {
            #region images.
            "1.png",
            "2.png",
            "3.png"
            #endregion
        };

        private readonly int _repairPoints = 100;

        public PowerUpRepair(Core core)
            : base(core)
        {
            int imageIndex = HgRandom.Random.Next(0, 1000) % _imagePaths.Count();
            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
            _repairPoints *= imageIndex + 1;
        }

        public new void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            if (Intersects(_core.Player.Actor))
            {
                _core.Player.Actor.AddHitPoints(_repairPoints);
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
