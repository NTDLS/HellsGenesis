using System.Drawing;

namespace Si.Shared.ExtensionMethods
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
