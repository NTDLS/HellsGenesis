using AI2D.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine
{
    public class EngineDisplay
    {
        public Dictionary<Point, Quadrant> Quadrants { get; private set; } = new Dictionary<Point, Quadrant>();
        public Quadrant CurrentQuadrant { get; set; }
        public Point<double> BackgroundOffset { get; private set; } = new Point<double>(); //Offset of background, all cals must take into account.
        public FrameCounter GameLoopCounter { get; private set; } = new FrameCounter();
        public RectangleF VisibleBounds { get; private set; }
        public RectangleF DrawBounds { get; private set; }
        public Size OverDraw { get; private set; }
        public Size DrawSize { get; private set; }
        public Size VisibleSize { get; private set; }
        public Control DrawingSurface { get; private set; }

        public Point<double> RandomOnScreenLocation()
        {
            return new Point<double>(Utility.Random.Next(0, VisibleSize.Width), Utility.Random.Next(0, VisibleSize.Height));
        }

        public Point<double> RandomOffScreenLocation(int min = 100, int max = 500)
        {
            double x;
            double y;

            if (Utility.FlipCoin())
            {
                if (Utility.FlipCoin())
                {
                    x = -Utility.RandomNumber(min, max);
                    y = Utility.RandomNumber(0, VisibleSize.Height);
                }
                else
                {
                    y = -Utility.RandomNumber(min, max);
                    x = Utility.RandomNumber(0, VisibleSize.Width);
                }
            }
            else
            {
                if (Utility.FlipCoin())
                {
                    x = VisibleSize.Width + Utility.RandomNumber(min, max);
                    y = Utility.RandomNumber(0, VisibleSize.Height);
                }
                else
                {
                    y = VisibleSize.Height + Utility.RandomNumber(min, max);
                    x = Utility.RandomNumber(0, VisibleSize.Width);
                }

            }

            return new Point<double>(x, y);
        }

        public EngineDisplay(Control drawingSurface, Size visibleSize)
        {
            DrawingSurface = drawingSurface;
            VisibleSize = visibleSize;

            int overdrawHeight = (int)(visibleSize.Height * 0.30);
            int overdrawWidth = (int)(visibleSize.Width * 0.30);
            if (overdrawHeight % 2 != 0) overdrawHeight++;
            if (overdrawWidth % 2 != 0) overdrawWidth++;
            OverDraw = new Size(overdrawWidth, overdrawHeight);

            DrawSize = new Size(visibleSize.Width + OverDraw.Width, visibleSize.Height + OverDraw.Height);
            DrawBounds = new RectangleF(0, 0, DrawSize.Width, DrawSize.Height);
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
