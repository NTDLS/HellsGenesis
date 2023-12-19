using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Engine.Types.Geometry;

namespace StrikeforceInfinity.Game.Sprites
{
    internal class SpriteRadarPositionTextBlock : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(EngineCore core, TextFormat format, SolidColorBrush color, SiPoint location)
            : base(core, format, color, location, false)
        {
            Visable = false;
            Velocity = new HgVelocity();
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
