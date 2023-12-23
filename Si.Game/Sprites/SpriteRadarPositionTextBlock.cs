using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Game.Engine;
using Si.Game.Engine.Types;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;

namespace Si.Game.Sprites
{
    internal class SpriteRadarPositionTextBlock : SpriteTextBlock
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
