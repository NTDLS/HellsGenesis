using System.Drawing;

namespace AI2D.Types
{
    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD()
        {
        }

        public PointD(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public PointD(PointD p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }

        public PointD(PointF p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }

        public PointD(Point p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }
    }
}
