using HG.Engine;
using HG.Engine.Types;
using HG.Engine.Types.Geometry;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace HG.Sprites
{
    internal class ActorRadarPositionTextBlock : ActorTextBlock
    {
        public ActorRadarPositionTextBlock(EngineCore core, TextFormat format, SolidColorBrush color, HgPoint location)
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
