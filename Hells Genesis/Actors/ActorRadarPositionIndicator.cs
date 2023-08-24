using HG.Engine;
using System.Drawing;
using System.IO;

namespace HG.Actors
{
    internal class ActorRadarPositionIndicator : ActorBase
    {
        private const string _assetPath = @"..\..\..\Assets\Graphics\Radar Indicator\";
        private readonly string _assetFile = "8x8.png";

        public ActorRadarPositionIndicator(Core core)
            : base(core)
        {
            Initialize(Path.Combine(_assetPath, _assetFile), new Size(8, 8));

            X = 0;
            Y = 0;
        }
    }
}
