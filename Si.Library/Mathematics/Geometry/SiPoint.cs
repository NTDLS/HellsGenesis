using System.Drawing;

namespace Si.Library.Mathematics.Geometry
{
    /// <summary>
    /// Implements a basic vector.
    /// </summary>
    public partial class SiPoint
    {
        public static readonly SiPoint Zero = new();

        public float X;
        public float Y;

        #region ~Ctor. 

        public SiPoint() { }

        public SiPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public SiPoint(SiPoint p)
        {
            X = p.X;
            Y = p.Y;
        }

        #endregion

        public RectangleF ToRectangleF(float width, float height) => new(X, Y, width, height);
        public RectangleF ToRectangleF(SizeF size) => new(X, Y, size.Width, size.Height);
        public RectangleF ToRectangleF() => new(X, Y, 1f, 1f);

        #region Operator Overloads.

        public static SiPoint operator -(SiPoint original, SiPoint modifier)
            => new SiPoint(original.X - modifier.X, original.Y - modifier.Y);

        public static SiPoint operator -(SiPoint original, float modifier)
           => new SiPoint(original.X - modifier, original.Y - modifier);

        public static SiPoint operator +(SiPoint original, SiPoint modifier)
            => new SiPoint(original.X + modifier.X, original.Y + modifier.Y);

        public static SiPoint operator +(SiPoint original, float modifier)
            => new SiPoint(original.X + modifier, original.Y + modifier);

        public static SiPoint operator *(SiPoint original, SiPoint scaleFactor)
            => new SiPoint(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        public static SiPoint operator *(SiPoint original, float scaleFactor)
            => new SiPoint(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiPoint operator /(SiPoint original, SiPoint scaleFactor)
        {
            if (scaleFactor.X == 0.0 && scaleFactor.Y == 0.0)
            {
                return new SiPoint(0, 0);
            }
            return new SiPoint(original.X / scaleFactor.X, original.Y / scaleFactor.Y);
        }

        public static SiPoint operator /(SiPoint original, float scaleFactor)
        {
            if (scaleFactor == 0.0)
            {
                return new SiPoint(0, 0);
            }
            return new SiPoint(original.X / scaleFactor, original.Y / scaleFactor);
        }

        public override bool Equals(object? o)
            => Math.Round(((SiPoint?)o)?.X ?? float.NaN, 4) == X && Math.Round(((SiPoint?)o)?.Y ?? float.NaN, 4) == Y;

        #endregion

        #region IComparible.

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString()
            => $"{{{Math.Round(X, 4).ToString("#.####")},{Math.Round(Y, 4).ToString("#.####")}}}";

        #endregion

        public SiPoint Clone() => new SiPoint(this);

        /// <summary>
        /// Normalize a vector to have a length of 1 but maintain its direction. Useful for velocity or direction vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public SiPoint Normalize() => Normalize(this);

        /// <summary>
        /// Calculate the dot product of two vectors.This is useful for determining the angle between vectors or projecting one vector onto another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float DotProduct(SiPoint b) => DotProduct(this);

        /// <summary>
        /// Calculate the angle between two points relative to the horizontal axis.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public float AngleBetween(SiPoint point2) => AngleBetween(this, point2);

        public SiPoint Clamp(float minValue, float maxValue)
        {
            var point = Clone();

            if (point.X < minValue)
            {
                point.X = minValue;
            }
            else if (point.X > maxValue)
            {
                point.X = maxValue;
            }

            if (point.Y < minValue)
            {
                point.Y = minValue;
            }
            else if (point.Y > maxValue)
            {
                point.Y = maxValue;
            }

            return point;
        }
    }
}
