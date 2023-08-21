using AI2D.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D.Engine.Managers
{
    public class EngineDisplayManager
    {
        public const double OverdrawScale = 1;
        public Dictionary<Point, Quadrant> Quadrants { get; private set; } = new Dictionary<Point, Quadrant>();
        public Quadrant CurrentQuadrant { get; set; }
        public Point<double> BackgroundOffset { get; private set; } = new Point<double>(); //Offset of background, all cals must take into account.
        public FrameCounter GameLoopCounter { get; private set; } = new FrameCounter();
        public RectangleF VisibleBounds { get; private set; }
        //public RectangleF DrawBounds { get; private set; }
        public Size OverdrawSize { get; private set; }
        public Size DrawSize { get; private set; }

        /// <summary>
        /// The size of the screen with no scaling.
        /// </summary>
        public Size NatrualScreenSize { get; private set; }
        public Control DrawingSurface { get; private set; }
        public double ThrottleFrameScaleFactor { get; set; }
        public double BoostFrameScaleFactor { get; set; }
        public double TotalFrameScaleFactor => ThrottleFrameScaleFactor + BoostFrameScaleFactor;
        public int TotalFrameScaleSubtraction() => (int)(OverdrawSize.Width / 4 * (TotalFrameScaleFactor / 100));

        public RectangleF DrawBounds
        {
            get
            {
                return new RectangleF(OverdrawSize.Width / 2.0f, OverdrawSize.Height / 2.0f,
                        NatrualScreenSize.Width * 2f, NatrualScreenSize.Height * 2f
                );
            }
        }

        public Rectangle CurrentScreenBounds
        {
            get
            {
                int scaleSubtraction = TotalFrameScaleSubtraction();
                return new Rectangle(OverdrawSize.Width / 2 - scaleSubtraction, OverdrawSize.Height / 2 - scaleSubtraction,
                        NatrualScreenSize.Width + scaleSubtraction * 2, NatrualScreenSize.Height + scaleSubtraction * 2
                );
            }
        }

        public Size CurrentScreenSize
        {
            get
            {
                int scaleSubtraction = TotalFrameScaleSubtraction();
                return new Size(NatrualScreenSize.Width + scaleSubtraction * 2, NatrualScreenSize.Height + scaleSubtraction * 2);
            }
        }

        public Point<double> RandomOnScreenLocation()
        {
            return new Point<double>(Utility.Random.Next(0, NatrualScreenSize.Width), Utility.Random.Next(0, NatrualScreenSize.Height));
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
                    y = Utility.RandomNumber(0, NatrualScreenSize.Height);
                }
                else
                {
                    y = -Utility.RandomNumber(min, max);
                    x = Utility.RandomNumber(0, NatrualScreenSize.Width);
                }
            }
            else
            {
                if (Utility.FlipCoin())
                {
                    x = NatrualScreenSize.Width + Utility.RandomNumber(min, max);
                    y = Utility.RandomNumber(0, NatrualScreenSize.Height);
                }
                else
                {
                    y = NatrualScreenSize.Height + Utility.RandomNumber(min, max);
                    x = Utility.RandomNumber(0, NatrualScreenSize.Width);
                }

            }

            return new Point<double>(x, y);
        }

        public EngineDisplayManager(Control drawingSurface, Size visibleSize)
        {
            DrawingSurface = drawingSurface;
            NatrualScreenSize = visibleSize;

            int overdrawHeight = (int)(visibleSize.Height * OverdrawScale);
            int overdrawWidth = (int)(visibleSize.Width * OverdrawScale);
            if (overdrawHeight % 2 != 0) overdrawHeight++;
            if (overdrawWidth % 2 != 0) overdrawWidth++;
            OverdrawSize = new Size(overdrawWidth, overdrawHeight);

            DrawSize = new Size(visibleSize.Width + OverdrawSize.Width, visibleSize.Height + OverdrawSize.Height);
            //DrawBounds = new RectangleF(0, 0, DrawSize.Width, DrawSize.Height);
            VisibleBounds = new RectangleF(0, 0, visibleSize.Width, visibleSize.Height);
        }

        public Quadrant GetQuadrant(double x, double y)
        {
            var coord = new Point(
                    (int)(x / NatrualScreenSize.Width),
                    (int)(y / NatrualScreenSize.Height)
                );

            if (Quadrants.ContainsKey(coord) == false)
            {
                var absoluteBounds = new Rectangle(
                    NatrualScreenSize.Width * coord.X,
                    NatrualScreenSize.Height * coord.Y,
                    NatrualScreenSize.Width,
                    NatrualScreenSize.Height);

                var quad = new Quadrant(coord, absoluteBounds);

                Quadrants.Add(coord, quad);
            }

            return Quadrants[coord];
        }
    }
}
