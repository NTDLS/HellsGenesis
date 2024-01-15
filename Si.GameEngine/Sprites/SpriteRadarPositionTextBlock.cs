using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Sprites
{
    public class SpriteRadarPositionTextBlock : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(Core.Engine gameEngine, TextFormat format, SolidColorBrush color, SiPoint location)
            : base(gameEngine, format, color, location, false)
        {
            RenderScaleOrder = Shared.SiConstants.SiRenderScaleOrder.PreScale;
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
