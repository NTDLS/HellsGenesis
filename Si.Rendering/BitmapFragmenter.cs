using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using Si.Library;
using Si.Rendering.Types;
using System.Collections.Generic;

namespace Si.Rendering
{
    internal static class BitmapFragmenter
    {
        public static List<Bitmap> GenerateIrregularFragments(RenderingEngine rendering,
            Bitmap originalBitmap, int countOfFragments, int countOfVertices)
        {
            var fragments = new List<Bitmap>();

            int bitmapWidth = originalBitmap.PixelSize.Width;
            int bitmapHeight = originalBitmap.PixelSize.Height;

            // Extract the polygon region from the original bitmap for each fragment.
            for (int fragmentIndex = 0; fragmentIndex < countOfFragments; fragmentIndex++)
            {
                // Generate a random polygon.
                var vertices = new RawVector2[countOfVertices];
                for (int verticesIndex = 0; verticesIndex < countOfVertices; verticesIndex++)
                {
                    vertices[verticesIndex] = new RawVector2(SiRandom.Between(0, bitmapWidth), SiRandom.Between(0, bitmapHeight));
                }

                var polygon = new SiPolygon(vertices);

                // Clip the polygon to ensure it fits within the bounds of the original bitmap.
                polygon = polygon.Clip(SiRandom.Between(bitmapWidth / 4, bitmapWidth), SiRandom.Between(bitmapHeight / 4, bitmapHeight));

                var fragment = ExtractPolygonRegion(rendering, originalBitmap, polygon);

                fragments.Add(fragment);
            }

            return fragments;
        }

        private static Bitmap ExtractPolygonRegion(RenderingEngine rendering, Bitmap bitmap, SiPolygon polygon)
        {
            var bounds = polygon.GetBounds();

            // Calculate the width and height of the bounding rectangle.
            var width = (int)(bounds.Right - bounds.Left);
            var height = (int)(bounds.Bottom - bounds.Top);

            return rendering.RenderTargets.Use(o =>
            {
                using (var tmpTarget = new BitmapRenderTarget(o.ScreenRenderTarget, CompatibleRenderTargetOptions.None, new Size2F(width, height)))
                {
                    tmpTarget.BeginDraw();

                    using (var geometry = new PathGeometry(tmpTarget.Factory))
                    using (var sink = geometry.Open())
                    {
                        sink.BeginFigure(polygon.Vertices[0], FigureBegin.Filled);
                        sink.AddLines(polygon.Vertices);
                        sink.EndFigure(FigureEnd.Closed);
                        sink.Close();

                        // Create a layer to use as a mask.
                        using (var layer = new Layer(tmpTarget))
                        {
                            var layerProps = new LayerParameters
                            {
                                ContentBounds = new RawRectangleF(0, 0, width, height),
                                MaskTransform = Matrix3x2.Identity
                            };

                            tmpTarget.PushLayer(ref layerProps, layer);

                            // Fill the layer with the original bitmap.
                            tmpTarget.DrawBitmap(bitmap, bounds, 1.0f, BitmapInterpolationMode.Linear, bounds);

                            // Pop the layer to apply the mask.
                            tmpTarget.PopLayer();

                            tmpTarget.Clear(rendering.Materials.Colors.Transparent);

                            // Draw the clipped bitmap onto the temporary render target.
                            tmpTarget.DrawBitmap(bitmap, new RawRectangleF(0, 0, width, height), 1.0f, BitmapInterpolationMode.Linear, bounds);
                            tmpTarget.EndDraw();

                            return tmpTarget.Bitmap;
                        }
                    }
                }
            });
        }
    }
}
