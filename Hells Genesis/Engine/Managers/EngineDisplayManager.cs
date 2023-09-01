using HG.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HG.Engine.Managers
{
    internal class EngineDisplayManager
    {
        private readonly Core _core;

        public Dictionary<Point, HgQuadrant> Quadrants { get; private set; } = new();
        public HgQuadrant CurrentQuadrant { get; set; }
        public HgPoint<double> BackgroundOffset { get; private set; } = new HgPoint<double>(); //Offset of background, all cals must take into account.
        public FrameCounter GameLoopCounter { get; private set; } = new FrameCounter();

        public Control DrawingSurface { get; private set; }

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
        public int SpeedOrientedFrameScalingSubtractionX() => (int)(OverdrawSize.Width / 2.0 * ((ThrottleFrameScaleFactor + BoostFrameScaleFactor) / 100.0));
        public int SpeedOrientedFrameScalingSubtractionY() => (int)(OverdrawSize.Height / 2.0 * ((ThrottleFrameScaleFactor + BoostFrameScaleFactor) / 100.0));
        public double SpeedOrientedFrameScalingFactor() =>
            1 - (OverdrawSize.Width / 2.0 * ((ThrottleFrameScaleFactor + BoostFrameScaleFactor) / 100.0) / _core.Display.OverdrawSize.Width) / 2.0;

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
                int scaleSubtractionX = SpeedOrientedFrameScalingSubtractionX();
                int scaleSubtractionY = SpeedOrientedFrameScalingSubtractionY();
                return new RectangleF(OverdrawSize.Width / 2.0f - scaleSubtractionX, OverdrawSize.Height / 2.0f - scaleSubtractionY,
                        NatrualScreenSize.Width + scaleSubtractionX * 2.0f, NatrualScreenSize.Height + scaleSubtractionY * 2.0f);
            }
        }

        public Size CurrentScreenSize
        {
            get
            {
                int scaleSubtractionX = SpeedOrientedFrameScalingSubtractionX();
                int scaleSubtractionY = SpeedOrientedFrameScalingSubtractionY();
                return new Size(NatrualScreenSize.Width + scaleSubtractionX * 2, NatrualScreenSize.Height + scaleSubtractionY * 2);
            }
        }

        public HgPoint<double> RandomOnScreenLocation()
        {
            return new HgPoint<double>(HgRandom.Random.Next(0, NatrualScreenSize.Width), HgRandom.Random.Next(0, NatrualScreenSize.Height));
        }

        public HgPoint<double> RandomOffScreenLocation(int min = 100, int max = 500)
        {
            double x;
            double y;

            if (HgRandom.FlipCoin())
            {
                if (HgRandom.FlipCoin())
                {
                    x = -HgRandom.RandomNumber(min, max);
                    y = HgRandom.RandomNumber(0, NatrualScreenSize.Height);
                }
                else
                {
                    y = -HgRandom.RandomNumber(min, max);
                    x = HgRandom.RandomNumber(0, NatrualScreenSize.Width);
                }
            }
            else
            {
                if (HgRandom.FlipCoin())
                {
                    x = NatrualScreenSize.Width + HgRandom.RandomNumber(min, max);
                    y = HgRandom.RandomNumber(0, NatrualScreenSize.Height);
                }
                else
                {
                    y = NatrualScreenSize.Height + HgRandom.RandomNumber(min, max);
                    x = HgRandom.RandomNumber(0, NatrualScreenSize.Width);
                }

            }

            return new HgPoint<double>(x, y);
        }

        public EngineDisplayManager(Core core, Control drawingSurface, Size visibleSize)
        {
            _core = core;
            DrawingSurface = drawingSurface;
            NatrualScreenSize = visibleSize;

            int overdrawHeight = (int)(visibleSize.Height * core.Settings.OverdrawScale);
            int overdrawWidth = (int)(visibleSize.Width * core.Settings.OverdrawScale);
            if (overdrawHeight % 2 != 0) overdrawHeight++;
            if (overdrawWidth % 2 != 0) overdrawWidth++;
            OverdrawSize = new Size(overdrawWidth, overdrawHeight);

            TotalCanvasSize = new Size(visibleSize.Width + OverdrawSize.Width, visibleSize.Height + OverdrawSize.Height);
        }

        public HgQuadrant GetQuadrant(double x, double y)
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

                var quad = new HgQuadrant(coord, absoluteBounds);

                Quadrants.Add(coord, quad);
            }

            return Quadrants[coord];
        }
    }
}
