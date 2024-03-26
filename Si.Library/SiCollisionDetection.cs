using System.Drawing;

namespace Si.Library
{
    public class SiCollisionDetection
    {
        /// <summary>
        /// Determines if two (non-axis-aligned) rectangles interset using Separating Axis Theorem (SAT).
        /// </summary>
        /// <param name="bounds1"></param>
        /// <param name="angleRadians1"></param>
        /// <param name="bounds2"></param>
        /// <param name="angleRadians2"></param>
        /// <returns></returns>
        public static bool IntersectsRotated(RectangleF bounds1, float angleRadians1, RectangleF bounds2, float angleRadians2)
        {
            var corners1 = GetRotatedRectangleCorners(bounds1, angleRadians1);
            var corners2 = GetRotatedRectangleCorners(bounds2, angleRadians2);

            // For each rectangle, there are 2 axes to test (perpendicular to its 2 unique sides)
            for (int i = 0; i < 2; i++)
            {
                PointF axisA = Perpendicular(Normalize(Subtract(corners1[(i + 1) % 4], corners1[i])));
                PointF axisB = Perpendicular(Normalize(Subtract(corners2[(i + 1) % 4], corners2[i])));

                if (!Overlaps(corners1, corners2, axisA) || !Overlaps(corners1, corners2, axisB))
                {
                    return false; // No collision if there's no overlap on at least one axis
                }
            }

            return true; // Overlap on all tested axes, so rectangles intersect
        }

        public static PointF[] GetRotatedRectangleCorners(RectangleF bounds, float angleRadians)
        {
            var center = new PointF(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
            var corners = new PointF[4];

            // Rectangle corners before rotation
            corners[0] = new PointF(bounds.Left, bounds.Top); // Top-left
            corners[1] = new PointF(bounds.Right, bounds.Top); // Top-right
            corners[2] = new PointF(bounds.Right, bounds.Bottom); // Bottom-right
            corners[3] = new PointF(bounds.Left, bounds.Bottom); // Bottom-left

            for (int i = 0; i < 4; i++)
            {
                // Translate corner to origin
                float x = corners[i].X - center.X;
                float y = corners[i].Y - center.Y;

                // Rotate and translate back
                corners[i] = new PointF(
                    (float)(x * Math.Cos(angleRadians) - y * Math.Sin(angleRadians) + center.X),
                    (float)(x * Math.Sin(angleRadians) + y * Math.Cos(angleRadians) + center.Y)
                );
            }

            return corners;
        }

        // Subtract two points to get the vector
        public static PointF Subtract(PointF a, PointF b)
            => new(a.X - b.X, a.Y - b.Y);

        // Normalize a vector
        public static PointF Normalize(PointF v)
        {
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            return new PointF(v.X / length, v.Y / length);
        }

        // Get a vector that is perpendicular to the given vector
        public static PointF Perpendicular(PointF v) => new(-v.Y, v.X);

        // Check if projections of two rectangles on a given axis overlap
        public static bool Overlaps(PointF[] cornersA, PointF[] cornersB, PointF axis)
        {
            (float minA, float maxA) = Project(cornersA, axis);
            (float minB, float maxB) = Project(cornersB, axis);
            return minA <= maxB && minB <= maxA; // Check for overlap
        }

        // Project the corners of a rectangle onto an axis and return the min and max scalar values
        public static (float, float) Project(PointF[] corners, PointF axis)
        {
            float min = Dot(corners[0], axis);
            float max = min;
            for (int i = 1; i < corners.Length; i++)
            {
                float p = Dot(corners[i], axis);
                if (p < min) min = p;
                else if (p > max) max = p;
            }
            return (min, max);
        }

        // Dot product of two vectors
        public static float Dot(PointF a, PointF axis) => a.X * axis.X + a.Y * axis.Y;
    }
}
