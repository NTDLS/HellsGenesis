using Si.Engine.Sprites._Superclass;
using System.IO;

namespace Si.Engine.Sprites
{
    public class SpriteRadarPositionIndicator : SpriteBase
    {
        private const string _assetPath = @"Graphics\Radar Indicator\";
        private readonly string _assetFile = "16x16.png";

        public SpriteRadarPositionIndicator(EngineCore engine)
            : base(engine)
        {
            Initialize(Path.Combine(_assetPath, _assetFile));

            X = 0;
            Y = 0;
        }
    }
}
