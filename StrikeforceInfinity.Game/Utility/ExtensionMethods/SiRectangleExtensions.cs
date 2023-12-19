using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace StrikeforceInfinity.Game.Utility.ExtensionMethods
{
    internal static class SiRectangleExtensions
    {
        /// <summary>
        /// Clones a float rectangle.
        /// </summary>
        public static RectangleF Clone(this RectangleF rectangle)
        {
            return new RectangleF(rectangle.Location, rectangle.Size);
        }

        /// <summary>
        /// Converts a float rectangle to a raw float rectangle.
        /// </summary>
        public static RawRectangleF ToRawRectangleF(this RectangleF value)
        {
            return new RawRectangleF(value.Left, value.Top, value.Right, value.Bottom);
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
