using Si.Engine.Sprite._Superclass._Root;
using System.IO;

namespace Si.Engine.Sprite
{
    public class SpriteRadarPositionIndicator : SpriteBase
    {
        private const string _assetPath = @"Sprites\Radar Indicator\";
        private readonly string _assetFile = "16x16.png";

        public SpriteRadarPositionIndicator(EngineCore engine)
            : base(engine)
        {
            SetImage(Path.Combine(_assetPath, _assetFile));

            X = 0;
            Y = 0;
        }
    }
}
