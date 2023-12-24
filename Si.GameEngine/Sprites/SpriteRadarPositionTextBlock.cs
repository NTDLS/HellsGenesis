using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.GameEngine.Engine;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Sprites
{
    public class SpriteRadarPositionTextBlock : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(EngineCore gameCore, TextFormat format, SolidColorBrush color, SiPoint location)
            : base(gameCore, format, color, location, false)
        {
            Visable = false;
            Velocity = new SiVelocity();
        }

        private double _distanceValue;
        public double DistanceValue
        {
            get
            {
                return _distanceValue;
            }
            set
            {
                _distanceValue = value;
                Text = DistanceValue.ToString("#,#");
            }
        }
    }
}
