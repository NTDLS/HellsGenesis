using HG.Engine;
using HG.Types;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace HG.Actors.Ordinary
{
    internal class ActorRadarPositionTextBlock : ActorTextBlock
    {
        public ActorRadarPositionTextBlock(Core core, TextFormat format, SolidColorBrush color, HgPoint<double> location)
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
