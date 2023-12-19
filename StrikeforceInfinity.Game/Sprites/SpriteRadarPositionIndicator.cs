using StrikeforceInfinity.Game.Engine;
using System.IO;

namespace StrikeforceInfinity.Game.Sprites
{
    internal class SpriteRadarPositionIndicator : SpriteBase
    {
        private const string _assetPath = @"Graphics\Radar Indicator\";
        private readonly string _assetFile = "16x16.png";

        public SpriteRadarPositionIndicator(EngineCore gameCore)
            : base(gameCore)
        {
            Initialize(Path.Combine(_assetPath, _assetFile));

            X = 0;
            Y = 0;
        }
    }
}
