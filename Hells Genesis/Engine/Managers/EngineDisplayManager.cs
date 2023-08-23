using HG.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HG.Engine.Managers
{
    internal class EngineDisplayManager
    {
        private readonly Core _core;

        public Dictionary<Point, HGQuadrant> Quadrants { get; private set; } = new();
        public HGQuadrant CurrentQuadrant { get; set; }
        public HGPoint<double> BackgroundOffset { get; private set; } = new HGPoint<double>(); //Offset of background, all cals must take into account.
        public FrameCounter GameLoopCounter { get; private set; } = new FrameCounter();

        public Control DrawingSurface { get; private set; }

        /// <summary>
        /// How much larger then the screen (NatrualScreenSize) that we will make the canvas so we can zoom-out.
        /// </summary>
        public const double OverdrawScale = 1;

        /// <summary>
        /// The scaling factor to apply based on the current player speed.
        /// </summary>
        public double ThrottleFrameScaleFactor { get; set; }

        /// The scaling factor to apply based on the current player boost.
        public double BoostFrameScaleFactor { get; set; }

        /// <summary>
        /// The number of pixles to remove from the length and width to perform the scaling at the current speed.
        /// </summary>
        /// <returns></returns>
        public int SpeedOrientedFrameScalingSubtraction() => (int)(OverdrawSize.Width / 4.0 * ((ThrottleFrameScaleFactor + BoostFrameScaleFactor) / 100.0));

        /// <summary>
        /// The number of extra pixles to draw beyond the NatrualScreenSize.
        /// </summary>
        public Size OverdrawSize { get; private set; }

        /// <summary>
        /// The total size of the rendering surface (no scaling).
        /// </summary>
        public Size TotalCanvasSize { get; private set; }

        /// <summary>
        /// The size of the screen with no scaling.
        /// </summary>
        public Size NatrualScreenSize { get; private set; }

        /// <summary>
        /// The bounds of the screen with no scaling.
        /// </summary>
        public RectangleF NatrualScreenBounds
        {
            get
            {
                return new RectangleF(OverdrawSize.Width / 2.0f, OverdrawSize.Height / 2.0f,
                        NatrualScreenSize.Width, NatrualScreenSize.Height
                );
            }
        }

        /// <summary>
        /// The total bounds of the drawing surface (canvas) natrual + overdraw (with no scaling) with respect to the infinite scrolling background,
        /// </summary>
        public RectangleF VirtualTotalScreenBounds
        {
            get
            {
                return new RectangleF((float)_core.Display.BackgroundOffset.X, (float)_core.Display.BackgroundOffset.Y, TotalCanvasSize.Width, TotalCanvasSize.Height);
            }
        }

        /// <summary>
        /// The total bounds of the drawing surface (canvas) natrual + overdraw (with no scaling).
        /// </summary>
        public RectangleF TotalScreenBounds
        {
            get
            {
                return new RectangleF(0, 0, TotalCanvasSize.Width, TotalCanvasSize.Height
                );
            }
        }

        public RectangleF CurrentScaledScreenBounds
        {
            get
            {
                int scaleSubtraction = SpeedOrientedFrameScalingSubtraction();
                return new RectangleF(OverdrawSize.Width / 2.0f - scaleSubtraction, OverdrawSize.Height / 2.0f - scaleSubtraction,
                        NatrualScreenSize.Width + scaleSubtraction * 2.0f, NatrualScreenSize.Height + scaleSubtraction * 2.0f
                );
            }
        }

        public Size CurrentScreenSize
        {
            get
            {
                int scaleSubtraction = SpeedOrientedFrameScalingSubtraction();
                return new Size(NatrualScreenSize.Width + scaleSubtraction * 2, NatrualScreenSize.Height + scaleSubtraction * 2);
            }
        }

        public HGPoint<double> RandomOnScreenLocation()
        {
            return new HGPoint<double>(HGRandom.Random.Next(0, NatrualScreenSize.Width), HGRandom.Random.Next(0, NatrualScreenSize.Height));
        }

        public HGPoint<double> RandomOffScreenLocation(int min = 100, int max = 500)
        {
            double x;
            double y;

            if (HGRandom.FlipCoin())
            {
                if (HGRandom.FlipCoin())
                {
                    x = -HGRandom.RandomNumber(min, max);
                    y = HGRandom.RandomNumber(0, NatrualScreenSize.Height);
                }
                else
                {
                    y = -HGRandom.RandomNumber(min, max);
                    x = HGRandom.RandomNumber(0, NatrualScreenSize.Width);
                }
            }
            else
            {
                if (HGRandom.FlipCoin())
                {
                    x = NatrualScreenSize.Width + HGRandom.RandomNumber(min, max);
                    y = HGRandom.RandomNumber(0, NatrualScreenSize.Height);
                }
                else
                {
                    y = NatrualScreenSize.Height + HGRandom.RandomNumber(min, max);
                    x = HGRandom.RandomNumber(0, NatrualScreenSize.Width);
                }

            }

            return new HGPoint<double>(x, y);
        }

        public EngineDisplayManager(Core core, Control drawingSurface, Size visibleSize)
        {
            _core = core;
            DrawingSurface = drawingSurface;
            NatrualScreenSize = visibleSize;

            int overdrawHeight = (int)(visibleSize.Height * OverdrawScale);
            int overdrawWidth = (int)(visibleSize.Width * OverdrawScale);
            if (overdrawHeight % 2 != 0) overdrawHeight++;
            if (overdrawWidth % 2 != 0) overdrawWidth++;
            OverdrawSize = new Size(overdrawWidth, overdrawHeight);

            TotalCanvasSize = new Size(visibleSize.Width + OverdrawSize.Width, visibleSize.Height + OverdrawSize.Height);
        }

        public HGQuadrant GetQuadrant(double x, double y)
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

                var quad = new HGQuadrant(coord, absoluteBounds);

                Quadrants.Add(coord, quad);
            }

            return Quadrants[coord];
        }
    }
}
