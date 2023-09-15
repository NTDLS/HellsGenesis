using HG.Actors.BaseClasses;
using HG.Engine;
using System.IO;

namespace HG.Actors.Ordinary
{
    internal class ActorRadarPositionIndicator : ActorBase
    {
        private const string _assetPath = @"Graphics\Radar Indicator\";
        private readonly string _assetFile = "16x16.png";

        public ActorRadarPositionIndicator(EngineCore core)
            : base(core)
        {
            Initialize(Path.Combine(_assetPath, _assetFile));

            X = 0;
            Y = 0;
        }
    }
}
