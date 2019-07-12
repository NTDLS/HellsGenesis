using AI2D.Engine;
using System.Drawing;

namespace AI2D.GraphicObjects
{
    public class ObjRadarPositionTextBlock: ObjTextBlock
    {
        public ObjRadarPositionTextBlock(Core core, string font, Brush color, double size, double x, double y)
            : base(core, font, color, size, x, y, false)
        {
            Visable = false;
            Velocity = new Types.VelocityD();
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
