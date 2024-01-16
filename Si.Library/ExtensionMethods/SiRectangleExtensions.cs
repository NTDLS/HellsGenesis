using System.Drawing;

namespace Si.Library.ExtensionMethods
{
    public static class SiRectangleExtensions
    {
        /// <summary>
        /// Clones a float rectangle.
        /// </summary>
        public static RectangleF Clone(this RectangleF rectangle)
        {
            return new RectangleF(rectangle.Location, rectangle.Size);
        }

        /// <summary>
        /// Similar to Inflate but grows the rectangle by all dimensions including the position. Returns a resized clone.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static RectangleF Grow(this RectangleF rectangle, float size)
        {
            return new RectangleF(
                rectangle.Location.X - size / 2,
                rectangle.Location.Y - size / 2,
                rectangle.Size.Width + size / 2,
                rectangle.Size.Height + size / 2);
        }

        /// <summary>
        /// Determines if the rectangle is inside of another rectangle.
        /// </summary>
        public static bool IntersectsWith(this RectangleF reference, RectangleF with, float tolerance)
        {
            return with.X < reference.X + reference.Width + tolerance
                && reference.X < with.X + with.Width + tolerance
                && with.Y < reference.Y + reference.Height + tolerance
                && reference.Y < with.Y + with.Height + tolerance;
        }
    }
}
