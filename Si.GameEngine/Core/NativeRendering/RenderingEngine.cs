using NTDLS.Semaphore;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Si.GameEngine.Core.NativeRendering
{
    public class RenderingEngine : IDisposable
    {
        public class CriticalRenderTargets
        {
            public BitmapRenderTarget IntermediateRenderTarget { get; set; }
            public WindowRenderTarget ScreenRenderTarget { get; set; }
        }

        public PessimisticCriticalResource<CriticalRenderTargets> RenderTargets { get; private set; } = new();
        public PrecreatedMaterials Materials { get; private set; }
        public PrecreatedTextFormats TextFormats { get; private set; }

        private readonly GameEngineCore _gameEngine;
        private readonly SharpDX.Direct2D1.Factory _direct2dFactory = new(FactoryType.SingleThreaded);
        private readonly SharpDX.DirectWrite.Factory _directWriteFactory = new();
        private readonly ImagingFactory _wicFactory = new();

        public RenderingEngine(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;

            var presentOptions = PresentOptions.Immediately;
            var antialiasMode = AntialiasMode.Aliased;

            if (gameEngine.Settings.VerticalSync == true)
            {
                presentOptions = PresentOptions.None;
            }

            if (gameEngine.Settings.AntiAliasing == true)
            {
                antialiasMode = AntialiasMode.PerPrimitive;
            }

            var renderProp = new HwndRenderTargetProperties()
            {
                PresentOptions = presentOptions,
                Hwnd = gameEngine.Display.DrawingSurface.Handle,
                PixelSize = new Size2(gameEngine.Display.NatrualScreenSize.Width, gameEngine.Display.NatrualScreenSize.Height)
            };

            var intermediateRenderTargetSize = new Size2F(_gameEngine.Display.TotalCanvasSize.Width, _gameEngine.Display.TotalCanvasSize.Height);

            var pixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);

            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget = new WindowRenderTarget(_direct2dFactory, new RenderTargetProperties(pixelFormat), renderProp)
                {
                    AntialiasMode = antialiasMode
                };

                o.IntermediateRenderTarget = new BitmapRenderTarget(
                    o.ScreenRenderTarget, CompatibleRenderTargetOptions.None, intermediateRenderTargetSize)
                {
                    AntialiasMode = antialiasMode
                };

                Materials = new PrecreatedMaterials(o.ScreenRenderTarget);
                TextFormats = new PrecreatedTextFormats(_directWriteFactory);
            });

            var renderTargetProperties = new RenderTargetProperties(pixelFormat);
            var renderProperties = new HwndRenderTargetProperties
            {
                Hwnd = gameEngine.Display.DrawingSurface.Handle,
                PixelSize = new Size2(gameEngine.Display.NatrualScreenSize.Width, gameEngine.Display.NatrualScreenSize.Height),
                PresentOptions = PresentOptions.Immediately
            };
        }

        public void Dispose()
        {
            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget?.Dispose();
                o.ScreenRenderTarget?.Dispose();
            });
        }

        public void TransferWithZoom(BitmapRenderTarget intermediateRenderTarget, RenderTarget screenRenderTarget, float scale)
        {
            var sourceRect = GraphicsUtility.CalculateCenterCopyRectangle(intermediateRenderTarget.Size, scale);
            var destRect = new RawRectangleF(0, 0, _gameEngine.Display.NatrualScreenSize.Width, _gameEngine.Display.NatrualScreenSize.Height);
            screenRenderTarget.DrawBitmap(intermediateRenderTarget.Bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
        }

        public SharpDX.Direct2D1.Bitmap BitmapStreamToD2DBitmap(Stream stream)
        {
            using var decoder = new BitmapDecoder(_wicFactory, stream, DecodeOptions.CacheOnLoad);
            using var frame = decoder.GetFrame(0);
            using var converter = new FormatConverter(_wicFactory);

            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA);

            return RenderTargets.Use(o => SharpDX.Direct2D1.Bitmap.FromWicBitmap(o.ScreenRenderTarget, converter));
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, double x, double y, double angleRadians)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF((float)x, (float)y, (float)(x + bitmap.PixelSize.Width), (float)(y + bitmap.PixelSize.Height));
            if (angleRadians != 0) ApplyTransformAngle(renderTarget, destRect, angleRadians);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            if (angleRadians != 0) ResetTransform(renderTarget);
            return destRect;
        }

        /// Draws a bitmap from a specified location of a given size, to the the specified location.
        public RawRectangleF DrawBitmapAt(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap,
            double x, double y, double angleRadians, RawRectangleF sourceRect, Size2F destSize)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF((float)x, (float)y, (float)(x + destSize.Width), (float)(y + destSize.Height));
            if (angleRadians != 0) ApplyTransformAngle(renderTarget, destRect, angleRadians);
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
            if (angleRadians != 0) ResetTransform(renderTarget);
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
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);
            return new RawRectangleF((float)x, (float)y, (float)(x + textLayout.Metrics.Width), (float)(y + textLayout.Metrics.Height));
        }

        public SizeF GetTextSize(string text, SharpDX.DirectWrite.TextFormat format)
        {
            //We have to check the size with some ending characters becuase TextLayout() seems to want to trim the text before calculating the metrics.
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, $"[{text}]", format, float.MaxValue, float.MaxValue);
            using var spacerLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, "[]", format, float.MaxValue, float.MaxValue);
            return new SizeF(textLayout.Metrics.Width - spacerLayout.Metrics.Width, textLayout.Metrics.Height);
        }

        /// <summary>
        /// Draws text at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the text.</returns>
        public RawRectangleF DrawTextAt(RenderTarget renderTarget,
            double x, double y, double angleRadians, string text, SharpDX.DirectWrite.TextFormat format, SolidColorBrush brush)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);

            var textWidth = textLayout.Metrics.Width;
            var textHeight = textLayout.Metrics.Height;

            // Create a rectangle that fits the text
            var textRectangle = new RawRectangleF((float)x, (float)y, (float)(x + textWidth), (float)(y + textHeight));

            //DrawRectangleAt(renderTarget, textRectangle, 0, Materials.Raw.Blue, 0, 1);

            if (angleRadians != 0) ApplyTransformAngle(renderTarget, textRectangle, angleRadians);
            renderTarget.DrawText(text, format, textRectangle, brush);
            if (angleRadians != 0) ResetTransform(renderTarget);

            return textRectangle;
        }

        public void DrawLine(RenderTarget renderTarget,
            double startPointX, double startPointY, double endPointX, double endPointY,
            SolidColorBrush brush, double strokeWidth = 1)
        {
            var startPoint = new RawVector2((float)startPointX, (float)startPointY);
            var endPoint = new RawVector2((float)endPointX, (float)endPointY);

            renderTarget.DrawLine(startPoint, endPoint, brush, (float)strokeWidth);
        }

        /// <summary>
        /// Draws a color filled ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public Ellipse FillEllipseAt(RenderTarget renderTarget, double x, double y,
            double radiusX, double radiusY, Color4 color, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2((float)x, (float)y),
                RadiusX = (float)radiusX,
                RadiusY = (float)radiusY,
            };

            if (angleRadians != 0)
            {
                var destRect = new RawRectangleF((float)x, (float)y, (float)(x + radiusX), (float)(y + radiusY));
                ApplyTransformAngle(renderTarget, destRect, angleRadians);
            }

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.FillEllipse(ellipse, brush);

            if (angleRadians != 0) ResetTransform(renderTarget);

            return ellipse;
        }

        /// <summary>
        /// Draws a hollow ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public Ellipse HollowEllipseAt(RenderTarget renderTarget, double x, double y,
            double radiusX, double radiusY, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2((float)x, (float)y),
                RadiusX = (float)radiusX,
                RadiusY = (float)radiusY,
            };

            if (angleRadians != 0)
            {
                var destRect = new RawRectangleF((float)x, (float)y, (float)(x + radiusX), (float)(y + radiusY));
                ApplyTransformAngle(renderTarget, destRect, angleRadians);
            }

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawEllipse(ellipse, brush, strokeWidth);

            if (angleRadians != 0) ResetTransform(renderTarget);

            return ellipse;
        }

        public void HollowTriangleAt(RenderTarget renderTarget, double x, double y,
            double height, double width, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            // Define the points for the triangle
            var trianglePoints = new RawVector2[]
            {
                new RawVector2(0, (float)height),           // Vertex 1 (bottom-left)
                new RawVector2((float)width, (float)height), // Vertex 2 (bottom-right)
                new RawVector2((float)(width / 2), 0)      // Vertex 3 (top-center)
            };

            if (angleRadians != 0)
            {
                var destRect = new RawRectangleF((float)x, (float)y, (float)(x + width), (float)(y + height));
                ApplyTransformAngle(renderTarget, destRect, angleRadians);
            }

            // Create a PathGeometry and add the triangle to it
            var triangleGeometry = new PathGeometry(_direct2dFactory);
            using (GeometrySink sink = triangleGeometry.Open())
            {
                sink.BeginFigure(trianglePoints[0], FigureBegin.Filled);
                sink.AddLines(trianglePoints);
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }

            // Calculate the center of the triangle
            float centerX = (trianglePoints[0].X + trianglePoints[1].X + trianglePoints[2].X) / 3;
            float centerY = (trianglePoints[0].Y + trianglePoints[1].Y + trianglePoints[2].Y) / 3;

            // Calculate the adjustment needed to center the triangle at the desired position
            x -= centerX;
            y -= centerY;

            // Create a translation transform to move the triangle to the desired position
            renderTarget.Transform = new(
                1.0f, 0.0f,
                0.0f, 1.0f,
                (float)x, (float)y
            );

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawGeometry(triangleGeometry, brush, strokeWidth);

            if (angleRadians != 0) ResetTransform(renderTarget);
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangleAt(RenderTarget renderTarget, RawRectangleF rect,
            double angleRadians, RawColor4 color, double expand = 0, double strokeWidth = 1)
        {
            if (expand != 0)
            {
                rect.Left -= (float)expand;
                rect.Top -= (float)expand;
                rect.Bottom += (float)expand;
                rect.Right += (float)expand;
            }

            ApplyTransformAngle(renderTarget, rect, angleRadians);
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawRectangle(rect, brush, (float)strokeWidth);
            ResetTransform(renderTarget);

            return rect;
        }

        public List<SharpDX.Direct2D1.Bitmap> GenerateIrregularFragments(SharpDX.Direct2D1.Bitmap originalBitmap, int countOfFragments, int countOfVertices = 3)
            => BitmapFragmenter.GenerateIrregularFragments(this, originalBitmap, countOfFragments, countOfVertices);

        public RawMatrix3x2 GetScalingMatrix(double zoomFactor)
        {
            // Calculate the new center point (assuming dimensions are known)
            float centerX = _gameEngine.Display.TotalCanvasSize.Width / 2.0f;
            float centerY = _gameEngine.Display.TotalCanvasSize.Height / 2.0f;

            // Calculate the scaling transformation matrix
            var scalingMatrix = new RawMatrix3x2(
                (float)zoomFactor, 0,
                0, (float)zoomFactor,
                (float)(centerX * (1 - zoomFactor)),
                (float)(centerY * (1 - zoomFactor))
            );

            return scalingMatrix;
        }

        #region Native Transformations.

        public void ApplyTransformAngle(RenderTarget renderTarget,
            RawRectangleF rect, double angleRadians, Matrix3x2? existingMatrix = null)
        {
            float centerX = rect.Left + (rect.Right - rect.Left) / 2.0f;
            float centerY = rect.Top + (rect.Bottom - rect.Top) / 2.0f;

            var rotationMatrix = Matrix3x2.Rotation((float)angleRadians, new Vector2(centerX, centerY));

            if (existingMatrix != null)
            {
                rotationMatrix = Matrix3x2.Multiply(rotationMatrix, (Matrix3x2)existingMatrix);
            }

            renderTarget.Transform = rotationMatrix;
        }


        public void ApplyScaleTransform(RenderTarget renderTarget, float scale, Vector2 centerPoint)
        {
            // Create a scale matrix at the specified center point
            var scaleMatrix = Matrix3x2.Scaling(scale, scale, centerPoint);

            // Apply the scale transform to the render target
            renderTarget.Transform = scaleMatrix;
        }

        public void ApplyZoomAndPan(RenderTarget renderTarget, float scaleX, float scaleY, Vector2 panOffset, Vector2 zoomCenter)
        {
            // Create a scaling matrix around the zoom center
            Matrix3x2 scalingMatrix = Matrix3x2.Scaling(scaleX, scaleY, zoomCenter);

            // Create a translation matrix for panning
            Matrix3x2 translationMatrix = Matrix3x2.Translation(panOffset);

            // Combine the scaling and translation matrices
            Matrix3x2 combinedMatrix = Matrix3x2.Multiply(scalingMatrix, translationMatrix);

            // Apply the combined transform to the render target
            renderTarget.Transform = combinedMatrix;
        }

        public void ResetTransform(RenderTarget renderTarget)
            => renderTarget.Transform = Matrix3x2.Identity;

        #endregion
    }
}

