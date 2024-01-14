using Si.GameEngine.Engine;
using Si.Shared;
using Si.Shared.ExtensionMethods;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Si.GameEngine.Managers
{
    /// <summary>
    /// Various metrics related to display.
    /// </summary>
    public class EngineDisplayManager
    {
        private readonly EngineCore _gameCore;

        public Dictionary<Point, SiQuadrant> Quadrants { get; private set; } = new();

        /// <summary>
        /// The background offset is really nothing more than the X/Y travel distance of the local player.
        /// </summary>
        public SiPoint BackgroundOffset { get; private set; } = new();
        public SiFrameCounter GameLoopCounter { get; private set; } = new();
        public Control DrawingSurface { get; private set; }

        private bool _isFocused = false;
        public bool IsDrawingSurfaceFocused => _isFocused;
        public void SetIsFocused(bool isFocused) => _isFocused = isFocused;

        public double OverrideSpeedOrientedFrameScalingFactor { get; set; } = double.NaN;

        public double SpeedOrientedFrameScalingFactor()
        {
            if (OverrideSpeedOrientedFrameScalingFactor is not double.NaN)
            {
                return OverrideSpeedOrientedFrameScalingFactor;
            }

            double weightedThrottlePercent = (
                    _gameCore.Player.Sprite.Velocity.ThrottlePercentage * 0.60 //n-percent of the zoom is throttle.
                    + _gameCore.Player.Sprite.Velocity.BoostPercentage * 0.40  //n-percent of the zoom is boost.
                ).Box(0, 1);

            double remainingRatioZoom = 1 - BaseDrawScale;
            double debugFactor = remainingRatioZoom * weightedThrottlePercent;
            return BaseDrawScale + debugFactor;
        }

        public double BaseDrawScale => 100.0 / _gameCore.Settings.OverdrawScale / 100.0;

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
        public RectangleF NatrualScreenBounds =>
            new RectangleF(OverdrawSize.Width / 2.0f, OverdrawSize.Height / 2.0f, NatrualScreenSize.Width, NatrualScreenSize.Height);

        /// <summary>
        /// The total bounds of the drawing surface (canvas) natrual + overdraw (with no scaling).
        /// </summary>
        public RectangleF TotalCanvasBounds => new RectangleF(0, 0, TotalCanvasSize.Width, TotalCanvasSize.Height);

        public RectangleF GetCurrentScaledScreenBounds()
        {
            var scale = SpeedOrientedFrameScalingFactor();

            if (scale < -1 || scale > 1)
            {
                throw new ArgumentException("Scale must be in the range [-1, 1].");
            }

            float centerX = TotalCanvasSize.Width * 0.5f;
            float centerY = TotalCanvasSize.Height * 0.5f;

            float smallerWidth = (float)(TotalCanvasSize.Width * scale);
            float smallerHeight = (float)(TotalCanvasSize.Height * scale);

            float left = centerX - smallerWidth * 0.5f;
            float top = centerY - smallerHeight * 0.5f;
            float right = smallerWidth;
            float bottom = smallerHeight;

            if (scale >= 0)
            {
                return new RectangleF(left, top, right, bottom);
            }
            else
            {
                //TODO: Zoom-in is untested.
                return new RectangleF(right, bottom, left, top);
            }
        }

        public SiPoint RandomOnScreenLocation()
        {
            var currentScaledScreenBounds = GetCurrentScaledScreenBounds();

            return new SiPoint(
                    SiRandom.Generator.Next((int)currentScaledScreenBounds.Left, (int)(currentScaledScreenBounds.Left + currentScaledScreenBounds.Width)),
                    SiRandom.Generator.Next((int)currentScaledScreenBounds.Top, (int)(currentScaledScreenBounds.Top + currentScaledScreenBounds.Height))
                );
        }

        public SiPoint RandomOffScreenLocation(int minOffscreenDistance = 100, int maxOffscreenDistance = 500)
        {
            if (SiRandom.FlipCoin())
            {
                if (SiRandom.FlipCoin())
                {
                    return new SiPoint(
                        -SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        SiRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new SiPoint(
                        -SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        SiRandom.Between(0, TotalCanvasSize.Width));
                }
            }
            else
            {
                if (SiRandom.FlipCoin())
                {
                    return new SiPoint(
                        TotalCanvasSize.Width + SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        SiRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new SiPoint(
                        TotalCanvasSize.Height + SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                    SiRandom.Between(0, TotalCanvasSize.Width));
                }
            }
        }

        public EngineDisplayManager(EngineCore gameCore, Control drawingSurface, Size visibleSize)
        {
            _gameCore = gameCore;
            DrawingSurface = drawingSurface;
            NatrualScreenSize = visibleSize;

            int totalSizeX = (int)(visibleSize.Width * _gameCore.Settings.OverdrawScale);
            int totalSizeY = (int)(visibleSize.Height * _gameCore.Settings.OverdrawScale);

            if (totalSizeX % 2 != 0) totalSizeX++;
            if (totalSizeY % 2 != 0) totalSizeY++;

            TotalCanvasSize = new Size(totalSizeX, totalSizeY);

            OverdrawSize = new Size(totalSizeX - NatrualScreenSize.Width, totalSizeY - NatrualScreenSize.Height);
        }

        public SiQuadrant GetQuadrant(double x, double y)
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

                var quad = new SiQuadrant(coord, absoluteBounds);

                Quadrants.Add(coord, quad);
            }

            return Quadrants[coord];
        }
    }
}
