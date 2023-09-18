using HG.Engine;
using HG.Sprites.BaseClasses;
using System.IO;

namespace HG.Sprites
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
