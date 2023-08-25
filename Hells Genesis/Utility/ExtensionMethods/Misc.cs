using System.Drawing;

namespace HG.Utility.ExtensionMethods
{
    internal static class Misc
    {
        public static RectangleF Clone(this RectangleF rectangle)
        {
            return new RectangleF(rectangle.Location, rectangle.Size);
        }

        public static bool IntersectsWith(this RectangleF reference, RectangleF with, float tolerance)
        {
            return with.X < reference.X + reference.Width + tolerance
                && reference.X < with.X + with.Width + tolerance
                && with.Y < reference.Y + reference.Height + tolerance
                && reference.Y < with.Y + with.Height + tolerance;
        }

        public static double NormalizeDegrees(this double value)
        {
            return (value + 180) % 360 - 180;
        }
       
        public static bool IsBetween(this double value, double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                return value >= maxValue && value <= minValue;
            }
            return value >= minValue && value <= maxValue;
        }

        public static double Box(this double value, double minValue, double maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }

        /// <summary>
        /// Take a value divides it by two and makes it negative if it over a given threshold
        /// </summary>
        /// <param name="value"></param>
        /// <param name="at"></param>
        /// <returns></returns>
        public static double SplitToNegative(this double value, double threshold)
        {
            value /= 2.0;

            if (value > threshold)
            {
                value *= -1;
            }

            return value;
        }
    }
}