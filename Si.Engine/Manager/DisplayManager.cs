using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Si.Engine.Manager
{
    /// <summary>
    /// Various metrics related to display.
    /// </summary>
    public class DisplayManager
    {
        private readonly EngineCore _engine;

        public SiFrameCounter FrameCounter { get; private set; } = new();

        public Dictionary<Point, SiQuadrant> Quadrants { get; private set; } = new();

        /// <summary>
        /// The X,Y of the top left of the render window. This is the corner of the total
        /// canvas which includes offscreen locations when not zoomed out. The local player
        /// will be centered in this window and the window will moved with the players movements.
        /// This can be though of as the camera.
        /// </summary>
        public SiVector RenderWindowPosition { get; set; } = new();
        public Control DrawingSurface { get; private set; }
        public Screen Screen { get; private set; }

        public bool IsDrawingSurfaceFocused { get; set; } = false;
        public void SetIsDrawingSurfaceFocused(bool isFocused) => IsDrawingSurfaceFocused = isFocused;

        public float SpeedOrientedFrameScalingFactor()
        {
            float weightedThrottlePercent = (
                (_engine.Player.Sprite.MovementVector.Length() / _engine.Player.Sprite.Speed) * 0.8f //80% of zoom is standard velocity
                 + (_engine.Player.Sprite.Throttle <= 1 ? 1 : _engine.Player.Sprite.Throttle / _engine.Player.Sprite.MaxThrottle) * 0.2f //20% of the zoom will be the "boost".
                ).Clamp(0, 1);

            return BaseDrawScale + ((1 - BaseDrawScale) * weightedThrottlePercent);
        }

        public float BaseDrawScale => 100.0f / _engine.Settings.OverdrawScale / 100.0f;

        /// <summary>
        /// The number of extra pixles to draw beyond the NatrualScreenSize.
        /// </summary>
        public Size OverdrawSize { get; private set; }

        /// <summary>
        /// The total size of the rendering surface (no scaling).
        /// </summary>
        public Size TotalCanvasSize { get; private set; }

        public float TotalCanvasDiagonal { get; private set; }

        public SiVector CenterCanvas;
        public SiVector CenterOfCurrentScreen => RenderWindowPosition + CenterCanvas;


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

        public SiVector RandomOnScreenLocation()
        {
            var currentScaledScreenBounds = GetCurrentScaledScreenBounds();

            return new SiVector(
                    SiRandom.Between((int)currentScaledScreenBounds.Left, (int)(currentScaledScreenBounds.Left + currentScaledScreenBounds.Width)),
                    SiRandom.Between((int)currentScaledScreenBounds.Top, (int)(currentScaledScreenBounds.Top + currentScaledScreenBounds.Height))
                );
        }

        //TODO: Tesr and fix this.
        public SiVector RandomOffScreenLocation(int minOffscreenDistance = 100, int maxOffscreenDistance = 500)
        {
            if (SiRandom.FlipCoin())
            {
                if (SiRandom.FlipCoin())
                {
                    return new SiVector(
                        RenderWindowPosition.X + -SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + SiRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new SiVector(
                        RenderWindowPosition.X + SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + SiRandom.Between(0, TotalCanvasSize.Height));
                }
            }
            else
            {
                if (SiRandom.FlipCoin())
                {
                    return new SiVector(
                        RenderWindowPosition.X + TotalCanvasSize.Width + SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + SiRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new SiVector(
                        RenderWindowPosition.X + TotalCanvasSize.Width + SiRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        RenderWindowPosition.Y + -SiRandom.Between(0, TotalCanvasSize.Height));
                }
            }
        }

        public DisplayManager(EngineCore engine, Control drawingSurface)
        {
            _engine = engine;
            DrawingSurface = drawingSurface;
            NatrualScreenSize = new Size(drawingSurface.Width, drawingSurface.Height);

            Screen = Screen.FromHandle(drawingSurface.Handle);

            int totalSizeX = (int)(NatrualScreenSize.Width * _engine.Settings.OverdrawScale);
            int totalSizeY = (int)(NatrualScreenSize.Height * _engine.Settings.OverdrawScale);

            if (totalSizeX % 2 != 0) totalSizeX++;
            if (totalSizeY % 2 != 0) totalSizeY++;

            TotalCanvasSize = new Size(totalSizeX, totalSizeY);
            OverdrawSize = new Size(totalSizeX - NatrualScreenSize.Width, totalSizeY - NatrualScreenSize.Height);
            CenterCanvas = new SiVector(TotalCanvasSize.Width / 2.0f, TotalCanvasSize.Height / 2.0f);

            TotalCanvasDiagonal = (float)Math.Sqrt(TotalCanvasSize.Width * TotalCanvasSize.Width + TotalCanvasSize.Height * TotalCanvasSize.Height);
        }

        public SiQuadrant GetQuadrant(float x, float y)
        {
            var coord = new Point((int)(x / NatrualScreenSize.Width), (int)(y / NatrualScreenSize.Height));

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
