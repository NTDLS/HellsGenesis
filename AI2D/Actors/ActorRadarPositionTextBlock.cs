using AI2D.Engine;
using AI2D.Types;
using System.Drawing;

namespace AI2D.Actors
{
    internal class ActorRadarPositionTextBlock : ActorTextBlock
    {
        public ActorRadarPositionTextBlock(Core core, string font, Brush color, double size, Point<double> location)
            : base(core, font, color, size, location, false)
        {
            Visable = false;
            Velocity = new Velocity<double>();
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
