using NebulaSiege.Engine;
using System.IO;

namespace NebulaSiege.Sprites
{
    internal class SpriteRadarPositionIndicator : _SpriteBase
    {
        private const string _assetPath = @"Graphics\Radar Indicator\";
        private readonly string _assetFile = "16x16.png";

        public SpriteRadarPositionIndicator(EngineCore core)
            : base(core)
        {
            Initialize(Path.Combine(_assetPath, _assetFile));

            X = 0;
            Y = 0;
        }
    }
}
