using HG.Utility;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HG.Engine.ImageProcessing
{
    internal class DirectX
    {
        private readonly Core _core;

        public BitmapRenderTarget IntermediateRenderTarget { get; private set; }
        public WindowRenderTarget ScreenRenderTarget { get; private set; }
        public SharpDX.Direct2D1.Factory D2dfactory { get; private set; }
        public SharpDX.DirectWrite.Factory DirectWriteFactory { get; private set; }
        public double GlobalScale { get; set; } = 50.0f;
        public DirectXMaterials Materials { get; private set; }
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

            Materials = new DirectXMaterials(ScreenRenderTarget);
            TextFormats = new DirectXTextFormats(DirectWriteFactory);
        }

        public void Cleanup()
        {
            ScreenRenderTarget?.Dispose();
        }

        public void ApplyScaling(float scale)
        {
            var sourceRect = CalculateCenterCopyRectangle(IntermediateRenderTarget.Size, scale);
            var destRect = new RawRectangleF(0, 0, _core.Display.NatrualScreenSize.Width, _core.Display.NatrualScreenSize.Height);
            ScreenRenderTarget.DrawBitmap(IntermediateRenderTarget.Bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
        }

        public static RawRectangleF CalculateCenterCopyRectangle(Size2F largerSize, float percentage)
        {
            if (percentage < -1 || percentage > 1)
            {
                throw new ArgumentException("Percentage must be in the range [-1, 1].");
            }

            float centerX = largerSize.Width * 0.5f;
            float centerY = largerSize.Height * 0.5f;

            float smallerWidth = largerSize.Width * percentage;
            float smallerHeight = largerSize.Height * percentage;

            float left = centerX - (smallerWidth * 0.5f);
            float top = centerY - (smallerHeight * 0.5f);
            float right = left + smallerWidth;
            float bottom = top + smallerHeight;

            if (percentage >= 0)
            {
                return new RawRectangleF(left, top, right, bottom);
            }
            else
            {
                return new RawRectangleF(right, bottom, left, top);
            }
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path)
        {
            using (var image = System.Drawing.Image.FromFile(path))
            {
                // Create a WIC stream from the System.Drawing.Image
                using (var wicFactory = new ImagingFactory())
                using (var imageStream = new MemoryStream())
                {
                    image.Save(imageStream, ImageFormat.Png);
                    imageStream.Seek(0, SeekOrigin.Begin);

                    using (var decoder = new BitmapDecoder(wicFactory, imageStream, DecodeOptions.CacheOnLoad))
                    using (var converter = new FormatConverter(wicFactory))
                    {
                        converter.Initialize(decoder.GetFrame(0), SharpDX.WIC.PixelFormat.Format32bppPBGRA);

                        return SharpDX.Direct2D1.Bitmap.FromWicBitmap(ScreenRenderTarget, converter);
                    }
                }
            }
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path, int newWidth, int newHeight)
        {
            using (var image = System.Drawing.Image.FromFile(path))
            {
                var resizedImage = new System.Drawing.Bitmap(newWidth, newHeight);

                using (var g = Graphics.FromImage(resizedImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
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

                        return SharpDX.Direct2D1.Bitmap.FromWicBitmap(ScreenRenderTarget, converter);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, double x, double y, double angle)
        {
            var destRect = new RawRectangleF((float)x, (float)y, (float)(x + bitmap.PixelSize.Width), (float)(y + bitmap.PixelSize.Height));
            SetTransformAngle(renderTarget, destRect, angle);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            ResetTransform(renderTarget);
            return destRect;
        }

        /// Draws a bitmap from a specified location of a given size, to the the specified location.
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap,
            double x, double y, double angle, RawRectangleF sourceRect, Size2F destSize)
        {
            var destRect = new RawRectangleF((float)x, (float)y, (float)(x + destSize.Width), (float)(y + destSize.Height));
            SetTransformAngle(renderTarget, destRect, angle);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
            ResetTransform(renderTarget);
            return destRect;
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, double x, double y)
        {
            var destRect = new RawRectangleF((float)x, (float)y, (float)(x + bitmap.PixelSize.Width), (float)(y + bitmap.PixelSize.Height));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            return destRect;
        }

        public RawRectangleF GetTextRext(double x, double y, string text, SharpDX.DirectWrite.TextFormat format)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(DirectWriteFactory, text, format, float.MaxValue, float.MaxValue);
            return new RawRectangleF((float)x, (float)y, (float)(x + textLayout.Metrics.Width), (float)(y + textLayout.Metrics.Height));
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
        public RawRectangleF DrawTextAt(RenderTarget renderTarget, double x, double y, double angle, string text, SharpDX.DirectWrite.TextFormat format, SolidColorBrush brush)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(DirectWriteFactory, text, format, float.MaxValue, float.MaxValue);

            var textWidth = textLayout.Metrics.Width;
            var textHeight = textLayout.Metrics.Height;

            // Create a rectangle that fits the text
            var textRectangle = new RawRectangleF((float)x, (float)y, (float)(x + textWidth), (float)(y + textHeight));

            SetTransformAngle(renderTarget, textRectangle, angle);
            renderTarget.DrawText(text, format, textRectangle, brush);
            ResetTransform(renderTarget);

            return textRectangle;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public Ellipse FillEllipseAt(RenderTarget renderTarget, double x, double y, double radiusX, double radiusY, RawColor4 color)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2((float)x, (float)y),
                RadiusX = (float)radiusX,
                RadiusY = (float)radiusY,
            };

            renderTarget.FillEllipse(ellipse, new SolidColorBrush(renderTarget, color));

            return ellipse;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangleAt(RenderTarget renderTarget, RawRectangleF rect, double angle, RawColor4 color, double expand = 0, double strokeWidth = 1)
        {
            if (expand != 0)
            {
                rect.Left -= (float)expand;
                rect.Top -= (float)expand;
                rect.Bottom += (float)expand;
                rect.Right += (float)expand;
            }

            SetTransformAngle(renderTarget, rect, angle);
            renderTarget.DrawRectangle(rect, new SolidColorBrush(renderTarget, color), (float)strokeWidth);
            ResetTransform(renderTarget);

            return rect;
        }

        public void SetTransformAngle(RenderTarget renderTarget, RawRectangleF rect, double angle, RawMatrix3x2? existimMatrix = null)
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

        private RawMatrix3x2 GetScalingMatrix(double zoomFactor)
        {
            // Calculate the new center point (assuming your image dimensions are known)
            float centerX = _core.Display.TotalCanvasSize.Width / 2.0f;
            float centerY = _core.Display.TotalCanvasSize.Height / 2.0f;

            // Calculate the scaling transformation matrix
            var scalingMatrix = new RawMatrix3x2(
                (float)zoomFactor, 0,
                0, (float)zoomFactor,
                (float)(centerX * (1 - zoomFactor)),
                (float)(centerY * (1 - zoomFactor))
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
