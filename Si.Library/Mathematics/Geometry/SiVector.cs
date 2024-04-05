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

        #region Float first.

        public static SiVector operator -(SizeF modifier, SiVector original)
            => new SiVector(modifier.Width - original.X, -modifier.Height - original.Y);

        public static SiVector operator -(Size modifier, SiVector original)
            => new SiVector(modifier.Width - original.X, modifier.Height - original.Y);

        public static SiVector operator -(float modifier, SiVector original)
           => new SiVector(modifier - original.X, modifier - original.Y);

        public static SiVector operator +(float modifier, SiVector original)
            => new SiVector(original.X + modifier, original.Y + modifier);

        public static SiVector operator *(float scaleFactor, SiVector original)
            => new SiVector(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiVector operator /(float scaleFactor, SiVector original)
        {
            if (scaleFactor == 0.0)
            {
                return new SiVector(0, 0);
            }
            return new SiVector(scaleFactor / original.X, scaleFactor / original.Y);
        }


        #endregion

        public static SiVector operator -(SiVector original, SizeF modifier)
            => new SiVector(original.X - modifier.Width, original.Y - modifier.Height);

        public static SiVector operator -(SiVector original, Size modifier)
            => new SiVector(original.X - modifier.Width, original.Y - modifier.Height);

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

        public static bool operator >(SiVector v1, SiVector v2)
            => v1.Length() > v2.Length();

        public static bool operator <(SiVector v1, SiVector v2)
            => v1.Length() < v2.Length();

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
        public float Dot(SiVector b) => Dot(this, b);

        /// <summary>
        /// Gets the length of the a vector. This represents the distance from its tail (starting point) to its head (end point) in the vector space.
        /// It provides a measure of how "long" the vector is in the specified direction.
        /// The length also serves as the vector magnatude.
        /// </summary>
        /// <returns></returns>
        public float Length() => Length(this);

        /// <summary>
        /// Returns the X + Y;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float Sum() => Sum(this);

        /// <summary>
        /// Returns the Abs(X) + Abs(Y);
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float SumAbs() => SumAbs(this);

        /// <summary>
        /// Calculates the euclidean distance between two points in a 2D space (slower and precisie, but not compatible with DistanceSquaredTo(...)).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceTo(SiVector other) => DistanceTo(this, other);

        /// <summary>
        /// Calculates the distance squared between two points in a 2D space (faster and but not compatible with DistanceTo(...)).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double DistanceSquaredTo(SiVector other) => DistanceSquaredTo(this, other);

        /// <summary>
        /// Calculate the angle between two points relative to the horizontal axis.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public float AngleBetween(SiVector point2) => AngleBetween(this, point2);

        public float AngleInDegreesTo360(SiVector point2) => AngleInDegreesTo360(this, point2);

        public float AngleToInSignedRadians(SiVector point2) => AngleToInSignedRadians(this, point2);


        public SiVector Clamp(float minValue, float maxValue)
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

        /// <summary>
        /// Rotates the given vector by the given radians.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public SiVector Rotate(float radians) => Rotate(this, radians);
    }
}
