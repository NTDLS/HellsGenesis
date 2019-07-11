using AI2D.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class EngineDisplay
    {
        public Dictionary<Point, Quadrant> Quadrants = new Dictionary<Point, Quadrant>();
        public Quadrant CurrentQuadrant { get; set; }
        public PointD BackgroundOffset { get; set; } = new PointD(); //Offset of background, all cals must take into account.
        public FrameCounter GameLoopCounter { get; set; } = new FrameCounter();
        public RectangleF VisibleBounds { get; private set; }

        private Size _visibleSize;
        public Size VisibleSize
        {
            get
            {
                return _visibleSize;
            }
        }

        private Control _drawingSurface;
        public Control DrawingSurface
        {
            get
            {
                return _drawingSurface;
            }
        }

        public PointD RandomOnScreenLocation()
        {
            return new PointD(Utility.Random.Next(0, VisibleSize.Width), Utility.Random.Next(0, VisibleSize.Height));
        }

        public PointD RandomOffScreenLocation(int min = 100, int max = 500)
        {
            double x;
            double y;

            if (Utility.FlipCoin())
            {
                x = -Utility.RandomNumber(100, 500);
            }
            else
            {
                x = VisibleSize.Width + Utility.RandomNumber(100, 500);
            }


            if (Utility.FlipCoin())
            {
                y = -Utility.RandomNumber(100, 500);
            }
            else
            {
                y = VisibleSize.Height + Utility.RandomNumber(100, 500);
            }
            return new PointD(x, y);
        }

        public EngineDisplay(Control drawingSurface, Size visibleSize)
        {
            _drawingSurface = drawingSurface;
            _visibleSize = visibleSize;
            VisibleBounds = new RectangleF(0, 0, visibleSize.Width, visibleSize.Height);
        }

        public Quadrant GetQuadrant(double x, double y)
        {
            var coord = new Point(
                    (int)(x / VisibleSize.Width),
                    (int)(y / VisibleSize.Height)
                );

            if (Quadrants.ContainsKey(coord) == false)
            {
                var absoluteBounds = new Rectangle(
                    VisibleSize.Width * coord.X,
                    VisibleSize.Height * coord.Y,
                    VisibleSize.Width,
                    VisibleSize.Height);

                var quad = new Quadrant(coord, absoluteBounds);

                Quadrants.Add(coord, quad);
            }

            return Quadrants[coord];
        }
    }
}
