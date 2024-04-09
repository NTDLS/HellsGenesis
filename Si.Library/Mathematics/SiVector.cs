using Si.Library.Sprite;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Si.Library.Mathematics
{
    /// <summary>
    /// 2d vector.
    /// </summary>
    public partial class SiVector : IComparable<SiVector>
    {
        public static readonly SiVector Zero = new();
        public static readonly SiVector UnitOfX = new(1f, 0f);
        public static readonly SiVector UnitOfY = new(0f, 1f);
        public static readonly SiVector One = new(1f, 1f);

        public float X;
        public float Y;

        #region ~Ctor. 

        public SiVector()
        {
        }

        public SiVector(float radians)
        {
            RadiansSigned = radians;
        }


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

        #region Converters.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF(float width, float height) => new(X, Y, width, height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF(SizeF size) => new(X, Y, size.Width, size.Height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF() => new(X, Y, 1f, 1f);

        /// <summary>
        /// Returns an SiVector from an angle in degrees.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector FromDegrees(float angleInDegrees) => new(SiMath.DegToRad(angleInDegrees));

        /// <summary>
        /// Returns an SiVector from an angle in degrees.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector FromRadians(float angleInRadians) => new(angleInRadians);

        /// <summary>
        /// Returns an SiVector from an angle in degrees.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector FromCardinal(float x, float y) => new(x, y);

        #endregion

        #region Operator Overloads: Float first.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(float scalar, SiVector original)
           => new SiVector(scalar - original.X, scalar - original.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator +(float scalar, SiVector original)
            => new SiVector(original.X + scalar, original.Y + scalar);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator *(float scaleFactor, SiVector original)
            => new SiVector(original.X * scaleFactor, original.Y * scaleFactor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator /(float scaleFactor, SiVector original)
        {
            if (scaleFactor == 0.0)
            {
                return new SiVector(0, 0);
            }
            return new SiVector(scaleFactor / original.X, scaleFactor / original.Y);
        }

        #endregion

        #region Operator Overloads: Float Second.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(SiVector original, float scalar)
           => new SiVector(original.X - scalar, original.Y - scalar);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator +(SiVector original, float scalar)
            => new SiVector(original.X + scalar, original.Y + scalar);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator *(SiVector original, float scaleFactor)
            => new SiVector(original.X * scaleFactor, original.Y * scaleFactor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator /(SiVector original, float scaleFactor)
            => scaleFactor == 0 ? Zero : new SiVector(original.X / scaleFactor, original.Y / scaleFactor);

        #endregion

        #region Operator Overloads: SizeF.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(SizeF modifier, SiVector original)
            => new SiVector(modifier.Width - original.X, -modifier.Height - original.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(SiVector original, SizeF modifier)
            => new SiVector(original.X - modifier.Width, original.Y - modifier.Height);

        #endregion

        #region Operator Overloads: Size.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(Size modifier, SiVector original)
            => new SiVector(modifier.Width - original.X, modifier.Height - original.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(SiVector original, Size modifier)
            => new SiVector(original.X - modifier.Width, original.Y - modifier.Height);

        #endregion

        #region Operator Overloads: Vector -> Vector.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SiVector? left, SiVector? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SiVector? left, SiVector? right)
            => !(left == right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(SiVector original, SiVector modifier)
            => new SiVector(original.X - modifier.X, original.Y - modifier.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator +(SiVector original, SiVector modifier)
            => new SiVector(original.X + modifier.X, original.Y + modifier.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator *(SiVector original, SiVector scaleFactor)
            => new SiVector(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(SiVector v1, SiVector v2)
            => v1.Magnitude() > v2.Magnitude();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(SiVector v1, SiVector v2)
            => v1.Magnitude() < v2.Magnitude();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator /(SiVector original, SiVector scaleFactor)
            => scaleFactor.X == 0.0 && scaleFactor.Y == 0.0 ? One :
                new SiVector(original.X / scaleFactor.X, original.Y / scaleFactor.Y);

        #endregion

        #region IComparible.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => ToString().GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => $"{{{Math.Round(X, 4).ToString("#.####")},{Math.Round(Y, 4).ToString("#.####")}}}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? o)
            => Math.Round(((SiVector?)o)?.X ?? float.NaN, 4) == X && Math.Round(((SiVector?)o)?.Y ?? float.NaN, 4) == Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(SiVector? other)
        {
            if (other == null) return 1; // Consider this instance greater if other is null

            // Calculate the magnitudes
            var thisMagnitude = Math.Sqrt(X * X + Y * Y);
            var otherMagnitude = Math.Sqrt(other.X * other.X + other.Y * other.Y);

            // Use the magnitudes to determine ordering
            return thisMagnitude.CompareTo(otherMagnitude);
        }

        #endregion

        #region Direction.

        public float RadiansUnsigned
        {
            get
            {
                var angle = RadiansSigned;
                if (angle < 0)
                {
                    angle += 2 * (float)Math.PI; // Convert negative angles to positive by adding 2π
                }
                return angle;
            }
            set
            {
                var radians = value > 0 ? value % SiMath.RADS_IN_CIRCLE : (value + SiMath.RADS_IN_CIRCLE) % SiMath.RADS_IN_CIRCLE;
                var cardinal = SiMath.RadToCardinal(radians);
                X = cardinal.X;
                Y = cardinal.Y;
            }
        }

        /// <summary>
        /// Angle in radians between [−3.14,3.14]
        /// </summary>
        public float RadiansSigned
        {
            get => SiMath.CardinalToRad(X, Y);
            set
            {
                var radians = value > 0 ? value % SiMath.RADS_IN_CIRCLE : (value + SiMath.RADS_IN_CIRCLE) % SiMath.RADS_IN_CIRCLE;
                var cardinal = SiMath.RadToCardinal(radians);
                X = cardinal.X;
                Y = cardinal.Y;
            }
        }

        public float DegreesUnsigned
        {
            get
            {
                float angleRadians = SiMath.CardinalToRad(X, Y);

                float angleDegrees = angleRadians * (180 / (float)Math.PI);
                if (angleDegrees < 0)
                {
                    angleDegrees += 360;
                }

                return angleDegrees;
            }
            set
            {
                var degrees = value > 0 ? value % 360 : (value + 360) % 360;
                var cardinal = SiMath.RadToCardinal(SiMath.DegToRad(degrees));
                X = cardinal.X;
                Y = cardinal.Y;
            }
        }

        /// <summary>
        /// Angle in degrees between [−180,180]
        /// </summary>
        public float DegreesSigned
        {
            get
            {
                float angleRadians = SiMath.CardinalToRad(X, Y);
                return angleRadians * (180 / (float)Math.PI);
            }
            set
            {
                var degrees = value > 0 ? value % 360 : (value + 360) % 360;
                var cardinal = SiMath.RadToCardinal(SiMath.DegToRad(degrees));
                X = cardinal.X;
                Y = cardinal.Y;
            }
        }

        #endregion

        /// <summary>
        /// Returns the clone of this vector.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiVector Clone()
            => new SiVector(X, Y);

        /// <summary>
        /// Rotates the vector by the given radians.
        /// </summary>
        /// <param name="angleRadians"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(float angleRadians)
        {
            var cosTheta = (float)Math.Cos(angleRadians);
            var sinTheta = (float)Math.Sin(angleRadians);

            var x = X * cosTheta - Y * sinTheta;
            var y = X * sinTheta + Y * cosTheta;

            X = x;
            Y = y;
        }

        /// <summary>
        /// Rotates the vector to the given radians while maintaining its length.
        /// </summary>
        /// <param name="angleRadians"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDirctionMaintainMagnitude(float angleRadians)
        {
            float magnitude = Magnitude();
            X = magnitude * (float)Math.Cos(angleRadians);
            Y = magnitude * (float)Math.Sin(angleRadians);
        }

        /// <summary>
        /// Returns a normalized vector, with a length of 1 but maintain its direction. Useful for velocity or direction vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiVector Normalize()
        {
            var magnitude = (float)Math.Sqrt(X * X + Y * Y);
            return new SiVector(X / magnitude, Y / magnitude);
        }

        /// <summary>
        /// Determines whether the vector is normalized.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNormalized()
            => SiMath.IsOne(X * X + Y * Y);

        /// <summary>
        /// Calculate the dot product of two vectors.This is useful for determining the angle between vectors or projecting one vector onto another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(SiVector vector)
             => X * vector.X + Y * vector.Y;

        /// <summary>
        /// Gets the length of the a vector. This represents the distance from its tail (starting point) to its head (end point) in the vector space.
        /// It provides a measure of how "long" the vector is in the specified direction.
        /// The length also serves as the vector magnatude.
        /// </summary>
        /// <altmember cref="LengthSquared"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude()
            => (float)Math.Sqrt(X * X + Y * Y);

        /// <summary>
        /// The length squared of a vector is the dot product of the vector with itself.
        /// This is useful for determining the angle between vectors or projecting one vector onto another.
        /// The length squared of a vector is the dot product of the vector with itself, and it's often used in optimizations where the actual
        /// distance (magnitude) isn't necessary. Calculating the square root (as in the magnitude) is computationally expensive, so using
        /// length squared can save resources when comparing distances or checking thresholds.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float LengthSquared()
            => X * X + Y * Y;

        /// <summary>
        /// Returns the X + Y;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Sum()
            => X + Y;

        /// <summary>
        /// Returns the Abs(X) + Abs(Y), useful for determining when a vector is non-zero.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SumAbs()
            => Math.Abs(X) + Math.Abs(Y);

        /// <summary>
        /// Calculates the euclidean distance between two points in a 2D space (slower and precisie, but not compatible with DistanceSquaredTo(...)).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DistanceTo(SiVector to)
        {
            var deltaX = Math.Pow(to.X - X, 2);
            var deltaY = Math.Pow(to.Y - Y, 2);
            return (float)Math.Sqrt(deltaY + deltaX);
        }

        /// <summary>
        /// Calculates the distance squared between two points in a 2D space (faster and but not compatible with DistanceTo(...)).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DistanceSquaredTo(SiVector to)
        {
            var deltaX = to.X - X;
            var deltaY = to.Y - Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Calculates a point at the normalized vector angle and a given distance. The instance vector will be normalized before calulation.
        /// </summary>
        /// <param name="distance">The distance to the given angle the point should be at.</param>
        /// <returns>The calculated point at the given distance towards the given angle.</returns>
        public SiVector PointFromAngleAtDistance(SiVector distance)
        {
            var direction = Normalize().RadiansSigned;
            return new SiVector((float)Math.Cos(direction) * distance.X, (float)Math.Sin(direction) * distance.Y);
        }

        /// <summary>
        /// Returns the delta angle from this to another expressed in degrees from 180--180, positive
        /// figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object,
        /// positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngle(SiVector toLocation, float offsetAngle = 0)
        {
            var angle = DeltaAngleInUnsignedDegrees(toLocation, offsetAngle);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Returns the delta angle from this vector to another expressed in degrees from 0-360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object,
        /// positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngleInUnsignedDegrees(SiVector toLocation, float offsetAngle = 0)
        {
            float fromAngle = DegreesUnsigned + offsetAngle;

            float angleTo = this.AngleToInUnsignedDegrees(toLocation);

            if (fromAngle < 0) fromAngle = 0 - fromAngle;
            if (angleTo < 0)
            {
                angleTo = 0 - angleTo;
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0f - Math.Abs(angleTo) % 360.0f;
            }

            return angleTo;
        }

        #region Sprite Math.

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAway(ISprite at, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngleInUnsignedDegrees(at));
            return deltaAngle < 180 + toleranceDegrees && deltaAngle > 180 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the object is pointing AWAY another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <param name="maxDistance"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAway(ISprite at, float toleranceDegrees, float maxDistance)
            => IsPointingAway(at, toleranceDegrees) && DistanceTo(at.Location) <= maxDistance;

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAt(ISprite at, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle(at));
            return deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees">The angle in degrees to consider the object to pointing at the other.</param>
        /// <param name="maxDistance">The distance in consider the object to pointing at the other.</param>
        /// <param name="offsetAngle">The offset in 0-360 degrees of the angle to calculate. For instance, 90 would tell if the right side of the object is pointing at the other.</param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointingAt(ISprite at, float toleranceDegrees, float maxDistance, float offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(DeltaAngleInUnsignedDegrees(at, offsetAngle));
            if (deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees)
            {
                return DistanceTo(at.Location) <= maxDistance;
            }

            return false;
        }

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 180--180, positive figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngle(ISprite to, float offsetAngle = 0)
        {
            var angle = DeltaAngleInUnsignedDegrees(to, offsetAngle);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 0-360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float DeltaAngleInUnsignedDegrees(ISprite to, float offsetAngle = 0)
        {
            float fromAngle = DegreesUnsigned + offsetAngle;

            float angleTo = this.AngleToInUnsignedDegrees(to.Location);

            if (fromAngle < 0) fromAngle = 0 - fromAngle;
            if (angleTo < 0)
            {
                angleTo = 0 - angleTo;
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0f - Math.Abs(angleTo) % 360.0f;
            }

            return angleTo;
        }

        #endregion
    }
}
