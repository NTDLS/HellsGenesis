using System.Drawing;

namespace Si.Library.Mathematics.Geometry
{
    /// <summary>
    /// Implements a basic vector.
    /// </summary>
    public partial class SiVector
    {
        public static readonly SiVector Zero = new();

        public float X;
        public float Y;

        #region ~Ctor. 

        public SiVector() { }

        public SiVector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public SiVector(SiVector p)
        {
            X = p.X;
            Y = p.Y;
        }

        #endregion

        public RectangleF ToRectangleF(float width, float height) => new(X, Y, width, height);
        public RectangleF ToRectangleF(SizeF size) => new(X, Y, size.Width, size.Height);
        public RectangleF ToRectangleF() => new(X, Y, 1f, 1f);

        #region Operator Overloads.

        public static SiVector operator -(SiVector original, SiVector modifier)
            => new SiVector(original.X - modifier.X, original.Y - modifier.Y);

        public static SiVector operator -(SiVector original, float modifier)
           => new SiVector(original.X - modifier, original.Y - modifier);

        public static SiVector operator +(SiVector original, SiVector modifier)
            => new SiVector(original.X + modifier.X, original.Y + modifier.Y);

        public static SiVector operator +(SiVector original, float modifier)
            => new SiVector(original.X + modifier, original.Y + modifier);

        public static SiVector operator *(SiVector original, SiVector scaleFactor)
            => new SiVector(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        public static SiVector operator *(SiVector original, float scaleFactor)
            => new SiVector(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiVector operator /(SiVector original, SiVector scaleFactor)
        {
            if (scaleFactor.X == 0.0 && scaleFactor.Y == 0.0)
            {
                return new SiVector(0, 0);
            }
            return new SiVector(original.X / scaleFactor.X, original.Y / scaleFactor.Y);
        }

        public static SiVector operator /(SiVector original, float scaleFactor)
        {
            if (scaleFactor == 0.0)
            {
                return new SiVector(0, 0);
            }
            return new SiVector(original.X / scaleFactor, original.Y / scaleFactor);
        }

        public override bool Equals(object? o)
            => Math.Round(((SiVector?)o)?.X ?? float.NaN, 4) == X && Math.Round(((SiVector?)o)?.Y ?? float.NaN, 4) == Y;

        #endregion

        #region IComparible.

        public override int GetHashCode() => ToString().GetHashCode();

        public override string ToString()
            => $"{{{Math.Round(X, 4).ToString("#.####")},{Math.Round(Y, 4).ToString("#.####")}}}";

        #endregion

        public SiVector Clone() => new SiVector(this);

        /// <summary>
        /// Normalize a vector to have a length of 1 but maintain its direction. Useful for velocity or direction vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public SiVector Normalize() => Normalize(this);

        /// <summary>
        /// Calculate the dot product of two vectors.This is useful for determining the angle between vectors or projecting one vector onto another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float DotProduct(SiVector b) => DotProduct(this);

        /// <summary>
        /// Calculate the angle between two points relative to the horizontal axis.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public float AngleBetween(SiVector point2) => AngleBetween(this, point2);
    }
}
