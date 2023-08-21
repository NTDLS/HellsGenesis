using System.Drawing;

namespace AI2D.Types
{
    public static class ExtensionsMethods
    {
        public static RectangleF Clone(this RectangleF rectangle)
        {
            return new RectangleF(rectangle.Location, rectangle.Size);
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