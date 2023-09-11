using HG.Utility.ExtensionMethods;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HG.Engine
{
    internal class DirectX
    {
        private Core _core;

        private readonly Dictionary<string, SharpDX.Direct2D1.Bitmap> _textureCache = new();
        public BitmapRenderTarget IntermediateRenderTarget { get; private set; }
        public WindowRenderTarget ScreenRenderTarget { get; private set; }
        public SharpDX.Direct2D1.Factory D2dfactory { get; private set; }
        public SharpDX.DirectWrite.Factory DirectWriteFactory { get; private set; }

        public float GlobalScale { get; set; } = 50.0f;

        public DirectXColors Colors { get; private set; }
        public DirectXTextFormats TextFormats { get; private set; }

        public DirectX(Core core)
        {
            _core = core;

            D2dfactory = new SharpDX.Direct2D1.Factory(FactoryType.SingleThreaded);

            var pixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
            var renderTargetProperties = new RenderTargetProperties(pixelFormat);
            var renderProperties = new HwndRenderTargetProperties
            {
                Hwnd = core.Display.DrawingSurface.Handle,
                //PixelSize = new Size2(core.Display.TotalCanvasSize.Width, core.Display.TotalCanvasSize.Height),
                PixelSize = new Size2(core.Display.NatrualScreenSize.Width, core.Display.NatrualScreenSize.Height),
                PresentOptions = PresentOptions.Immediately
            };

            ScreenRenderTarget = new WindowRenderTarget(D2dfactory, renderTargetProperties, renderProperties)
            {
                AntialiasMode = AntialiasMode.PerPrimitive,
                TextAntialiasMode = TextAntialiasMode.Cleartype
            };

            var intermediateRenderTargetSize = new Size2F(_core.Display.TotalCanvasSize.Width, _core.Display.TotalCanvasSize.Height);
            IntermediateRenderTarget = new BitmapRenderTarget(ScreenRenderTarget, CompatibleRenderTargetOptions.None, intermediateRenderTargetSize)
            {
                AntialiasMode = AntialiasMode.PerPrimitive,
                TextAntialiasMode = TextAntialiasMode.Cleartype
            };


            DirectWriteFactory = new SharpDX.DirectWrite.Factory();

            Colors = new DirectXColors(ScreenRenderTarget);
            TextFormats = new DirectXTextFormats(DirectWriteFactory);
        }

        public void Cleanup()
        {
            ScreenRenderTarget?.Dispose();
        }

        public void ApplyScaling()
        {
            GlobalScale = GlobalScale.Box(0, 100);

            float widthScale = _core.Display.OverdrawSize.Width * (GlobalScale / 100.0f);
            float heightScale = _core.Display.OverdrawSize.Height * (GlobalScale / 100.0f);

            // Define the source rectangle to crop from intermediateRenderTarget (assuming you want to crop from the center)
            RawRectangleF sourceRect = new RawRectangleF(widthScale, heightScale,
                IntermediateRenderTarget.Size.Width - widthScale, IntermediateRenderTarget.Size.Height - heightScale);

            var destRect = new RawRectangleF(0, 0, _core.Display.NatrualScreenSize.Width, _core.Display.NatrualScreenSize.Height);
            ScreenRenderTarget.DrawBitmap(IntermediateRenderTarget.Bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
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

                var result = SharpDX.Direct2D1.Bitmap.FromWicBitmap(ScreenRenderTarget, converter);

                _textureCache.Add(path, result);
                return result;
            }
        }

        public SharpDX.Direct2D1.Bitmap GetCachedBitmap(string path, int newWidth, int newHeight)
        {
            path = path.ToLower();

            lock (_textureCache)
            {
                if (_textureCache.TryGetValue(path, out var cached))
                {
                    return cached;
                }
                using (var image = System.Drawing.Image.FromFile(path)) // Replace 'yourStream' with your actual stream
                {
                    var resizedImage = new System.Drawing.Bitmap(newWidth, newHeight);

                    using (var g = Graphics.FromImage(resizedImage))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(image, 0, 0, newWidth, newHeight);
                    }

                    // Create a WIC stream from the System.Drawing.Image
                    using (var wicFactory = new ImagingFactory())
                    using (var imageStream = new MemoryStream())
                    {
                        resizedImage.Save(imageStream, ImageFormat.Png);
                        imageStream.Seek(0, SeekOrigin.Begin);

                        using (var decoder = new BitmapDecoder(wicFactory, imageStream, DecodeOptions.CacheOnLoad))
                        using (var converter = new FormatConverter(wicFactory))
                        {
                            converter.Initialize(decoder.GetFrame(0), SharpDX.WIC.PixelFormat.Format32bppPBGRA);

                            var result = SharpDX.Direct2D1.Bitmap.FromWicBitmap(ScreenRenderTarget, converter);

                            _textureCache.Add(path, result);

                            return result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y, float angle)
        {
            SetTransformAngle(renderTarget, new SharpDX.RectangleF(x, y, bitmap.PixelSize.Width, bitmap.PixelSize.Height), angle);

            var destRect = new RawRectangleF(x, y, x + bitmap.PixelSize.Width, y + bitmap.PixelSize.Height);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            ResetTransform(renderTarget);

            return destRect;
        }

        /// Draws a bitmap from a specified location of a given size, to the the specified location.
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y, float angle, RawRectangleF sourceRect, Size2F destSize)
        {
            var destRect = new RawRectangleF(x, y, x + destSize.Width, y + destSize.Height);
            SetTransformAngle(renderTarget, new SharpDX.RectangleF(x, y, destSize.Width, destSize.Height), angle);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
            ResetTransform(renderTarget);
            return destRect;
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y)
        {
            var destRect = new RawRectangleF(x, y, x + bitmap.PixelSize.Width, y + bitmap.PixelSize.Height);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            return destRect;
        }

        public RawRectangleF GetTextRext(float x, float y, string text, SharpDX.DirectWrite.TextFormat format)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(DirectWriteFactory, text, format, float.MaxValue, float.MaxValue);
            return new RawRectangleF(x, y, x + textLayout.Metrics.Width, y + textLayout.Metrics.Height);
        }

        public SizeF GetTextSize(string text, SharpDX.DirectWrite.TextFormat format)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(DirectWriteFactory, text, format, float.MaxValue, float.MaxValue);
            return new SizeF(textLayout.Metrics.Width, textLayout.Metrics.Height);
        }

        /// <summary>
        /// Draws text at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the text.</returns>
        public RawRectangleF DrawTextAt(RenderTarget renderTarget, float x, float y, float angle, string text, SharpDX.DirectWrite.TextFormat format, SolidColorBrush brush)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(DirectWriteFactory, text, format, float.MaxValue, float.MaxValue);

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
        public Ellipse FillEllipseAt(RenderTarget renderTarget, float x, float y, float radiusX, float radiusY, RawColor4 color)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            renderTarget.FillEllipse(ellipse, new SolidColorBrush(renderTarget, color));

            return ellipse;
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
            angle = HgMath.DegreesToRadians(angle);

            float centerX = rect.Left + rect.Width / 2.0f;
            float centerY = rect.Top + rect.Height / 2.0f;

            // Calculate the rotation matrix
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            var rotationMatrix = new RawMatrix3x2(
                cosAngle, sinAngle,
                -sinAngle, cosAngle,
                centerX - centerX * cosAngle + centerY * sinAngle,
                centerY - centerX * sinAngle - centerY * cosAngle
            );

            if (existimMatrix != null)
            {
                rotationMatrix = MultiplyMatrices((RawMatrix3x2)existimMatrix, rotationMatrix);
            }

            renderTarget.Transform = rotationMatrix;
        }

        public void SetTransformAngle(RenderTarget renderTarget, RawRectangleF rect, float angle, RawMatrix3x2? existimMatrix = null)
        {
            angle = HgMath.DegreesToRadians(angle);

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
