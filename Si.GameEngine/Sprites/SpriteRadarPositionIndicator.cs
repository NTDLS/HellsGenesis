using Si.GameEngine.Sprites._Superclass;
using System.IO;

namespace Si.GameEngine.Sprites
{
    public class SpriteRadarPositionIndicator : SpriteBase
    {
        private const string _assetPath = @"Graphics\Radar Indicator\";
        private readonly string _assetFile = "16x16.png";

        public SpriteRadarPositionIndicator(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            Initialize(Path.Combine(_assetPath, _assetFile));

            X = 0;
            Y = 0;
        }
    }
}
