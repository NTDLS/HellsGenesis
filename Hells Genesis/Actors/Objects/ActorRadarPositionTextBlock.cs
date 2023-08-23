using HG.Engine;
using HG.Types;
using System.Drawing;

namespace HG.Actors.Objects
{
    internal class ActorRadarPositionTextBlock : ActorTextBlock
    {
        public ActorRadarPositionTextBlock(Core core, string font, Brush color, double size, HGPoint<double> location)
            : base(core, font, color, size, location, false)
        {
            Visable = false;
            Velocity = new HGVelocity<double>();
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
