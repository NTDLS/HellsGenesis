using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HG.Engine
{
    internal class EngineD2Dx
    {
        private Core _core;

        private readonly Dictionary<string, SharpDX.Direct2D1.Bitmap> _textureCache = new();

        public WindowRenderTarget ScreenRenderTarget { get; private set; }
        public SharpDX.Direct2D1.Factory D2dfactory { get; private set; }
        public SharpDX.DirectWrite.Factory DirectWriteFactory { get; private set; }
        public TextFormat LargeTextFormat { get; private set; }

        #region Raw colors.

        public readonly RawColor4 RawColorRed = new RawColor4(1, 0, 0, 1);
        public readonly RawColor4 RawColorGreen = new RawColor4(0, 1, 0, 1);
        public readonly RawColor4 RawColorBlue = new RawColor4(0, 0, 1, 1);
        public readonly RawColor4 RawColorBlack = new RawColor4(0, 0, 0, 1);
        public readonly RawColor4 RawColorWhite = new RawColor4(1, 1, 1, 1);
        public readonly RawColor4 RawColorGray = new RawColor4(0.25f, 0.25f, 0.25f, 1);

        #endregion

        #region Solid brushes.

        public SolidColorBrush SolidColorBrushRed { get; private set; }
        public SolidColorBrush SolidColorBrushGreen { get; private set; }
        public SolidColorBrush SolidColorBrushBlue { get; private set; }
        public SolidColorBrush SolidColorBrushBlack { get; private set; }
        public SolidColorBrush SolidColorBrushWhite { get; private set; }
        public SolidColorBrush SolidColorBrushGray { get; private set; }
        #endregion

        public EngineD2Dx(Core core)
        {
            _core = core;

            D2dfactory = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.SingleThreaded);

            var pixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
            var renderTargetProperties = new RenderTargetProperties(pixelFormat);
            var renderProperties = new HwndRenderTargetProperties
            {
                Hwnd = core.Display.DrawingSurface.Handle,
                //PixelSize = new Size2(core.Display.TotalCanvasSize.Width, core.Display.TotalCanvasSize.Height),
                PixelSize = new Size2(core.Display.NatrualScreenSize.Width, core.Display.NatrualScreenSize.Height),
                PresentOptions = PresentOptions.Immediately
            };

            ScreenRenderTarget = new WindowRenderTarget(D2dfactory, renderTargetProperties, renderProperties);

            DirectWriteFactory = new SharpDX.DirectWrite.Factory();
            SolidColorBrushRed = new SolidColorBrush(ScreenRenderTarget, RawColorRed);
            SolidColorBrushGreen = new SolidColorBrush(ScreenRenderTarget, RawColorGreen);
            SolidColorBrushBlue = new SolidColorBrush(ScreenRenderTarget, RawColorBlue);
            SolidColorBrushBlack = new SolidColorBrush(ScreenRenderTarget, RawColorBlack);
            SolidColorBrushWhite = new SolidColorBrush(ScreenRenderTarget, RawColorWhite);
            SolidColorBrushGray = new SolidColorBrush(ScreenRenderTarget, RawColorGray);

            LargeTextFormat = new TextFormat(DirectWriteFactory, "Arial", 24);
        }

        public void Cleanup()
        {
            ScreenRenderTarget?.Dispose();
        }

        public SharpDX.Direct2D1.Bitmap GetCachedBitmap(RenderTarget renderTarget, string path)
        {
            path = path.ToLower();

            lock (_textureCache)
            {
                if (_textureCache.TryGetValue(path, out var cached))
                {
                    return cached;
                }

                using var wicFactory = new ImagingFactory();
                using var decoder = new BitmapDecoder(wicFactory, path, DecodeOptions.CacheOnLoad);
                using var converter = new FormatConverter(wicFactory);
                converter.Initialize(decoder.GetFrame(0), SharpDX.WIC.PixelFormat.Format32bppPBGRA);

                var result = SharpDX.Direct2D1.Bitmap.FromWicBitmap(renderTarget, converter);
                _textureCache.Add(path, result);
                return result;
            }
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y, float angle)
        {
            SetTransformAngle(renderTarget, new SharpDX.RectangleF(x, y, bitmap.PixelSize.Width, bitmap.PixelSize.Height), angle);

            // Apply the rotation matrix and draw the bitmap
            renderTarget.AntialiasMode = AntialiasMode.PerPrimitive; // Enable antialiasing

            var destRect = new RawRectangleF(x, y, x + bitmap.PixelSize.Width, y + bitmap.PixelSize.Height);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            ResetTransform(renderTarget);

            return destRect;
        }

        /// <summary>
        /// Draws text at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the text.</returns>
        public RawRectangleF DrawTextAt(RenderTarget renderTarget, float x, float y, float angle, string text, TextFormat format, SolidColorBrush brush)
        {
            var textLayout = new TextLayout(DirectWriteFactory, text, format, float.MaxValue, float.MaxValue);

            var textWidth = textLayout.Metrics.Width;
            var textHeight = textLayout.Metrics.Height;

            // Create a rectangle that fits the text
            var textRectangle = new RawRectangleF(x, y, x + textWidth, y + textHeight);

            SetTransformAngle(renderTarget, textRectangle, angle);
            renderTarget.DrawText(text, format, textRectangle, brush);
            ResetTransform(renderTarget);

            return textRectangle;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangleAt(RenderTarget renderTarget, RawRectangleF rect, float angle, RawColor4 color, float expand = 0, float strokeWidth = 1)
        {
            if (expand != 0)
            {
                rect.Left -= expand;
                rect.Top -= expand;
                rect.Bottom += expand;
                rect.Right += expand;
            }

            SetTransformAngle(renderTarget, rect, angle);
            renderTarget.DrawRectangle(rect, new SolidColorBrush(renderTarget, color), strokeWidth);
            ResetTransform(renderTarget);

            return rect;
        }

        public void SetTransformAngle(RenderTarget renderTarget, SharpDX.RectangleF rect, float angle, RawMatrix3x2? existimMatrix = null)
        {
            float centerX = rect.Left + rect.Width / 2.0f;
            float centerY = rect.Top + rect.Height / 2.0f;

            // Calculate the rotation matrix
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            var rotationMatrix = new RawMatrix3x2(
                cosAngle, sinAngle,
                -sinAngle, cosAngle,
                centerX - cosAngle * centerX + sinAngle * centerY,
                centerY - sinAngle * centerX - cosAngle * centerY
            );

            if (existimMatrix != null)
            {
                rotationMatrix = MultiplyMatrices((RawMatrix3x2)existimMatrix, rotationMatrix);
            }

            renderTarget.Transform = rotationMatrix;
        }

        public void SetTransformAngle(RenderTarget renderTarget, RawRectangleF rect, float angle, RawMatrix3x2? existimMatrix = null)
        {
            float centerX = rect.Left + (rect.Right - rect.Left) / 2.0f;
            float centerY = rect.Top + (rect.Bottom - rect.Top) / 2.0f;

            // Calculate the rotation matrix
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            var rotationMatrix = new RawMatrix3x2(
                cosAngle, sinAngle,
                -sinAngle, cosAngle,
                centerX - cosAngle * centerX + sinAngle * centerY,
                centerY - sinAngle * centerX - cosAngle * centerY
            );

            if (existimMatrix != null)
            {
                rotationMatrix = MultiplyMatrices(rotationMatrix, (RawMatrix3x2)existimMatrix);
            }

            renderTarget.Transform = rotationMatrix;
        }

        private RawMatrix3x2 GetScalingMatrix(float zoomFactor)
        {
            // Calculate the new center point (assuming your image dimensions are known)
            float centerX = _core.Display.TotalCanvasSize.Width / 2.0f;
            float centerY = _core.Display.TotalCanvasSize.Height / 2.0f;

            // Calculate the scaling transformation matrix
            var scalingMatrix = new RawMatrix3x2(
                zoomFactor, 0,
                0, zoomFactor,
                centerX * (1 - zoomFactor), centerY * (1 - zoomFactor)
            );

            return scalingMatrix;
        }

        /*
        public void FFS()
        {
            // Define the rotation angle in degrees
            float rotationAngleInDegrees = 45.0f; // Adjust as needed

            // Convert degrees to radians
            float rotationAngleInRadians = rotationAngleInDegrees * (float)Math.PI / 180.0f;

            // Create an intermediate render target for rendering bitmaps
            using (var intermediateRenderTarget = new BitmapRenderTarget(renderTarget, CompatibleRenderTargetOptions.None))
            {
                // Loop through your bitmaps and apply rotation to each one
                foreach (var bitmap in bitmaps)
                {
                    // Apply the rotation transformation to the current bitmap
                    renderTarget.Transform = new RawMatrix3x2(
                        (float)Math.Cos(rotationAngleInRadians), (float)Math.Sin(rotationAngleInRadians),
                        -(float)Math.Sin(rotationAngleInRadians), (float)Math.Cos(rotationAngleInRadians),
                        0, 0
                    );

                    // Draw the rotated bitmap onto the intermediate render target
                    intermediateRenderTarget.DrawBitmap(bitmap, new RawRectangleF(0, 0, bitmap.Width, bitmap.Height), 1.0f, BitmapInterpolationMode.Linear);
                }

                // Reset the transformation on the intermediate render target
                intermediateRenderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);

                // Calculate the crop percentage (e.g., 50% for center crop)
                float cropPercentage = 0.5f; // Adjust as needed

                // Calculate the scaling factor to achieve the desired crop percentage
                float scale = 1.0f / cropPercentage;

                // Apply the scaling transformation to crop the intermediate result
                var scalingMatrix = new RawMatrix3x2(
                    scale, 0,
                    0, scale,
                    0, 0
                );

                // Apply the scaling transformation to the entire intermediate scene
                intermediateRenderTarget.Transform = scalingMatrix;

                // Stretch the scaled image to fit the final render target (screen)
                var screenRect = new RawRectangleF(0, 0, screenWidth, screenHeight); // Adjust as needed
                renderTarget.DrawBitmap(intermediateRenderTarget.Bitmap, screenRect);

                // Reset the transformation on the final render target
                renderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
            }
        }
        */

        public RawMatrix3x2 MultiplyMatrices(RawMatrix3x2 matrix1, RawMatrix3x2 matrix2)
        {
            return new RawMatrix3x2(
                matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21,
                matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22,
                matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21,
                matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22,
                matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix2.M31,
                matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix2.M32
            );
        }

        public void ResetTransform(RenderTarget renderTarget)
        {
            renderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
        }
    }
}
