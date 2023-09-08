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

namespace HG
{
    internal class HgDirectX
    {
        private readonly Dictionary<string, SharpDX.Direct2D1.Bitmap> _textureCache = new();

        public WindowRenderTarget RenderTarget { get; private set; }
        public SharpDX.Direct2D1.Factory D2dfactory { get; private set; }
        public SharpDX.DirectWrite.Factory DirectWriteFactory { get; private set; }
        public TextFormat LargeTextFormat { get; private set; }

        public readonly RawColor4 RawColorRed = new RawColor4(1, 0, 0, 1);
        public readonly RawColor4 RawColorGreen = new RawColor4(0, 1, 0, 1);
        public readonly RawColor4 RawColorBlue = new RawColor4(0, 0, 1, 1);
        public readonly RawColor4 RawColorBlack = new RawColor4(0, 0, 0, 1);
        public readonly RawColor4 RawColorWhite = new RawColor4(1, 1, 1, 1);
        public readonly RawColor4 RawColorLightGray = new RawColor4(0.25f, 0.25f, 0.25f, 1);

        public SolidColorBrush SolidColorBrushRed { get; private set; }
        public SolidColorBrush SolidColorBrushGreen { get; private set; }
        public SolidColorBrush SolidColorBrushBlue { get; private set; }
        public SolidColorBrush SolidColorBrushBlack { get; private set; }
        public SolidColorBrush SolidColorBrushWhite { get; private set; }
        public SolidColorBrush SolidColorBrushLightGray { get; private set; }

        public HgDirectX(Form form)
        {
            D2dfactory = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.SingleThreaded);
            var renderProperties = new HwndRenderTargetProperties
            {
                Hwnd = form.Handle,
                PixelSize = new Size2(form.ClientSize.Width, form.ClientSize.Height),
                PresentOptions = PresentOptions.Immediately
            };

            RenderTarget = new WindowRenderTarget(D2dfactory, new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied)), renderProperties);

            DirectWriteFactory = new SharpDX.DirectWrite.Factory();
            SolidColorBrushRed = new SolidColorBrush(RenderTarget, RawColorRed);
            SolidColorBrushGreen = new SolidColorBrush(RenderTarget, RawColorGreen);
            SolidColorBrushBlue = new SolidColorBrush(RenderTarget, RawColorBlue);
            SolidColorBrushBlack = new SolidColorBrush(RenderTarget, RawColorBlack);
            SolidColorBrushWhite = new SolidColorBrush(RenderTarget, RawColorWhite);
            SolidColorBrushLightGray = new SolidColorBrush(RenderTarget, RawColorLightGray);

            LargeTextFormat = new TextFormat(DirectWriteFactory, "Arial", 24);
        }


        public SharpDX.Direct2D1.Bitmap GetCachedBitmap(string path)
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

                var result = SharpDX.Direct2D1.Bitmap.FromWicBitmap(RenderTarget, converter);
                _textureCache.Add(path, result);
                return result;
            }
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public RawRectangleF DrawBitmapAt(SharpDX.Direct2D1.Bitmap bitmap, float x, float y, float angle)
        {
            SetTransformAngle(new RectangleF(x, y, bitmap.PixelSize.Width, bitmap.PixelSize.Height), angle);

            // Apply the rotation matrix and draw the bitmap
            RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive; // Enable antialiasing

            var destRect = new RawRectangleF(x, y, x + bitmap.PixelSize.Width, y + bitmap.PixelSize.Height);
            RenderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            ResetTransform();

            return destRect;
        }

        /// <summary>
        /// Draws text at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the text.</returns>
        public RawRectangleF DrawTextAt(float x, float y, float angle, string text, TextFormat format, SolidColorBrush brush)
        {
            var textLayout = new TextLayout(DirectWriteFactory, text, format, float.MaxValue, float.MaxValue);

            var textWidth = textLayout.Metrics.Width;
            var textHeight = textLayout.Metrics.Height;

            // Create a rectangle that fits the text
            var textRectangle = new RawRectangleF(x, y, x + textWidth, y + textHeight);

            SetTransformAngle(textRectangle, angle);
            RenderTarget.DrawText(text, format, textRectangle, brush);
            ResetTransform();

            return textRectangle;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangleAt(RawRectangleF rect, float angle, RawColor4 color, float expand = 0, float strokeWidth = 1)
        {
            if (expand != 0)
            {
                rect.Left -= expand;
                rect.Top -= expand;
                rect.Bottom += expand;
                rect.Right += expand;
            }

            SetTransformAngle(rect, angle);
            RenderTarget.DrawRectangle(rect, new SolidColorBrush(RenderTarget, color), strokeWidth);
            ResetTransform();

            return rect;
        }

        public void SetTransformAngle(RectangleF rect, float angle)
        {
            float centerX = rect.Left + rect.Width / 2.0f;
            float centerY = rect.Top + rect.Height / 2.0f;

            // Calculate the rotation matrix
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            RenderTarget.Transform = new RawMatrix3x2(
                cosAngle, sinAngle,
                -sinAngle, cosAngle,
                centerX - cosAngle * centerX + sinAngle * centerY,
                centerY - sinAngle * centerX - cosAngle * centerY
            );
        }

        public void SetTransformAngle(RawRectangleF rect, float angle)
        {
            float centerX = rect.Left + (rect.Right - rect.Left) / 2.0f;
            float centerY = rect.Top + (rect.Bottom - rect.Top) / 2.0f;

            // Calculate the rotation matrix
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            RenderTarget.Transform = new RawMatrix3x2(
                cosAngle, sinAngle,
                -sinAngle, cosAngle,
                centerX - cosAngle * centerX + sinAngle * centerY,
                centerY - sinAngle * centerX - cosAngle * centerY
            );
        }

        public void ResetTransform()
        {
            RenderTarget.Transform = new RawMatrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
        }
    }
}
