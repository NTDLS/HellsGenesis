using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites
{
    public class SpriteRadarPositionTextBlock : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(GameEngineCore gameEngine, TextFormat format, SolidColorBrush color, SiPoint location)
            : base(gameEngine, format, color, location, false)
        {
            RenderScaleOrder = SiRenderScaleOrder.PreScale;
            Visable = false;
            Velocity = new SiVelocity();
        }

        private float _distanceValue;
        public float DistanceValue
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
