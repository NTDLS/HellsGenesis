using Si.Library.Sprite;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Si.Library.Mathematics.Geometry
{
    /// <summary>
    /// 2d vector and associated halpers.
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

        #region Converters.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF(float width, float height) => new(X, Y, width, height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF(SizeF size) => new(X, Y, size.Width, size.Height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectangleF ToRectangleF() => new(X, Y, 1f, 1f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiAngle ToAngle() => SiAngle.FromVector(this);

        /// <summary>
        /// Returns an SiVector from a SiAngle.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector FromAngle(SiAngle angle) => new(angle);

        /// <summary>
        /// Returns a normailized SiAngle from an SiVector.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle ToAngle(SiVector vector) => new(vector);

        #endregion

        #region Operator Overloads: Float first.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator -(float modifier, SiVector original)
           => new SiVector(modifier - original.X, modifier - original.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator +(float modifier, SiVector original)
            => new SiVector(original.X + modifier, original.Y + modifier);

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
        public static SiVector operator -(SiVector original, float modifier)
           => new SiVector(original.X - modifier, original.Y - modifier);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator +(SiVector original, float modifier)
            => new SiVector(original.X + modifier, original.Y + modifier);

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
        {
            return scaleFactor.X == 0.0 && scaleFactor.Y == 0.0 ? One :
                new SiVector(original.X / scaleFactor.X, original.Y / scaleFactor.Y);
        }

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

        #region Static Helpers.

        #region AngleTo...Degrees (Signed and Unsigned).

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(ISprite from, ISprite to)
            => AngleToInUnsignedDegrees(from.Location, to.Location);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 1-180 to -1-180.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(ISprite from, ISprite to)
        {
            var angle = AngleToInUnsignedDegrees(from.Location, to.Location);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Calculates the angle of one objects location to another location from 1-180 to -1-180.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(ISprite from, SiVector to)
        {
            var angle = AngleToInUnsignedDegrees(from.Location, to);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(SiVector from, ISprite to)
            => AngleToInUnsignedDegrees(from, to.Location);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(ISprite from, SiVector to)
            => AngleToInUnsignedDegrees(from.Location, to);

        /// <summary>
        /// Calculates the angle from one object to another, returns the degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(SiVector from, SiVector to)
        {
            var radians = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (SiMath.RadToDeg(radians) + 360.0f) % 360.0f;
        }

        #endregion

        #region AngleTo...Radians (Signed and Unsigned).

        /// <summary>
        /// Calculates the angle from one object to another, returns the radians.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedRadians(SiVector from, SiVector to)
            => (float)Math.Atan2(to.Y - from.Y, to.X - from.X);

        /// <summary>
        /// Calculate the angle between two points relative to the horizontal axis.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedRadians(SiVector point1, SiVector point2)
            => (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

        #endregion

        /// <summary>
        /// Returns a new vector which has been rotated by the given radians.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector Rotation(SiVector vector, float radians)
        {
            float cosTheta = (float)Math.Cos(radians);
            float sinTheta = (float)Math.Sin(radians);

            return new SiVector(
                vector.X * cosTheta - vector.Y * sinTheta,
                vector.X * sinTheta + vector.Y * cosTheta
            );
        }

        /// <summary>
        /// Calculates a point at a given angle and a given distance.
        /// </summary>
        /// <param name="angle">The angle which the point should move to in the range of 0-259.</param>
        /// <param name="distance">The distance to the given angle the point should be at.</param>
        /// <returns>The calculated point at the given distance towards the given angle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector PointFromAngleAtDistanceInUnsignedDegrees(SiAngle angle, SiVector distance)
            => new SiVector((float)Math.Cos(angle.Radians) * distance.X, (float)Math.Sin(angle.Radians) * distance.Y);

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAway(ISprite from, ISprite at, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngleInUnsignedDegrees(from, at));
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
        public static bool IsPointingAway(ISprite from, ISprite at, float toleranceDegrees, float maxDistance)
            => IsPointingAway(from, at, toleranceDegrees) && DistanceTo(from, at) <= maxDistance;

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAt(ISprite from, ISprite at, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle(from, at));
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
        public static bool IsPointingAt(ISprite from, ISprite at, float toleranceDegrees, float maxDistance, float offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(DeltaAngleInUnsignedDegrees(from, at, offsetAngle));
            if (deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees)
            {
                return DistanceTo(from, at) <= maxDistance;
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
        public static float DeltaAngle(ISprite from, ISprite to, float offsetAngle = 0)
        {
            var angle = DeltaAngleInUnsignedDegrees(from, to, offsetAngle);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 180--180, positive figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngle(ISprite from, SiVector toLocation, float offsetAngle = 0)
        {
            var angle = DeltaAngleInUnsignedDegrees(from, toLocation, offsetAngle);
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
        public static float DeltaAngleInUnsignedDegrees(ISprite from, ISprite to, float offsetAngle = 0)
        {
            float fromAngle = from.PointingAngle.Degrees + offsetAngle;

            float angleTo = AngleToInUnsignedDegrees(from, to);

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

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 0-360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngleInUnsignedDegrees(ISprite from, SiVector toLocation, float offsetAngle = 0)
        {
            float fromAngle = from.PointingAngle.Degrees + offsetAngle;

            float angleTo = AngleToInUnsignedDegrees(from, toLocation);

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

        /// <summary>
        /// Returns the distance from one object to another.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calcuated distance from one object to the other.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(ISprite from, ISprite to)
            => DistanceTo(from.Location, to.Location);

        /// <summary>
        /// Calculates the euclidean distance between two points in a 2D space (slower and precisie, but not compatible with DistanceSquaredTo(...)).
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(SiVector from, SiVector to)
        {
            var deltaX = Math.Pow(to.X - from.X, 2);
            var deltaY = Math.Pow(to.Y - from.Y, 2);
            return (float)Math.Sqrt(deltaY + deltaX);
        }

        /// <summary>
        /// Calculates the distance squared between two points in a 2D space (faster and but not compatible with DistanceTo(...)).
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquaredTo(SiVector from, SiVector to)
        {
            var deltaX = to.X - from.X;
            var deltaY = to.Y - from.Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

        /// <summary>
        /// Normalize a vector to have a length of 1 but maintain its direction. Useful for velocity or direction vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector Normalize(SiVector vector)
        {
            var magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            return new SiVector(vector.X / magnitude, vector.Y / magnitude);
        }

        /// <summary>
        /// Determines whether the vector is normalized.
        /// </summary>
        public static bool IsNormalized(SiVector vector)
            => SiMath.IsOne(vector.X * vector.X + vector.Y * vector.Y);

        /// <summary>
        /// Returns the X + Y;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sum(SiVector a) => a.X + a.Y;

        /// <summary>
        /// Returns the Abs(X) + Abs(Y);
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SumAbs(SiVector a) => Math.Abs(a.X) + Math.Abs(a.Y);

        /// <summary>
        /// Calculate the dot product of two vectors.This is useful for determining the angle between vectors or projecting one vector onto another.
        /// </summary>
        /// <altmember cref="LengthSquared"/>
        /// <altmember cref="Magnitude"/>
        /// <altmember cref="Magnitude"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(SiVector v1, SiVector v2) => v1.X * v2.X + v1.Y * v2.Y;

        /// <summary>
        /// The length squared of a vector is the dot product of the vector with itself.
        /// This is useful for determining the angle between vectors or projecting one vector onto another.
        /// The length squared of a vector is the dot product of the vector with itself, and it's often used in optimizations where the actual
        /// distance (magnitude) isn't necessary. Calculating the square root (as in the magnitude) is computationally expensive, so using
        /// length squared can save resources when comparing distances or checking thresholds.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float LengthSquared(SiVector v) => v.X * v.X + v.Y * v.Y;

        /// <summary>
        /// Gets the length of a vector from its tail to its head.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        /// <altmember cref="LengthSquared"/>
        /// <altmember cref="Magnitude"/>
        /// [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(SiVector vector)
            => (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

        /// <summary>
        /// Rotate a point around another point by a certain angle.
        /// </summary>
        /// <param name="pointToRotate"></param>
        /// <param name="centerPoint"></param>
        /// <param name="angleRadians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector RotateAroundPoint(SiVector pointToRotate, SiVector centerPoint, float angleRadians)
        {
            var cosTheta = (float)Math.Cos(angleRadians);
            var sinTheta = (float)Math.Sin(angleRadians);
            var x = cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X;
            var y = sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y;
            return new SiVector(x, y);
        }

        /// <summary>
        /// Reflect a vector off a surface.Useful for light reflections, bouncing effects, etc.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector Reflect(SiVector vector, SiVector normal)
        {
            var dotProduct = Dot(vector, normal);
            return new SiVector(vector.X - 2 * dotProduct * normal.X, vector.Y - 2 * dotProduct * normal.Y);
        }

        #endregion

        /// <summary>
        /// Returns the clone of this vector.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiVector Clone() => new SiVector(this);

        /// <summary>
        /// Returns a new vector which has been rotated by the given radians.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiVector Rotation(float radians) => Rotation(this, radians);

        /// <summary>
        /// Rotates the vector by the given radians.
        /// </summary>
        /// <param name="angleRadians"></param>
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
        /// Rotates the vector to the given radians.
        /// </summary>
        /// <param name="angleRadians"></param>
        public void RotateTo(float angleRadians)
        {
            float magnitude = Magnitude();
            X = magnitude * (float)Math.Cos(angleRadians);
            Y = magnitude * (float)Math.Sin(angleRadians);
        }

        /// <summary>
        /// Normalize a vector to have a length of 1 but maintain its direction. Useful for velocity or direction vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiVector Normalize() => Normalize(this);

        /// <summary>
        /// Calculate the dot product of two vectors.This is useful for determining the angle between vectors or projecting one vector onto another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(SiVector b) => Dot(this, b);

        /// <summary>
        /// Gets the length of the a vector. This represents the distance from its tail (starting point) to its head (end point) in the vector space.
        /// It provides a measure of how "long" the vector is in the specified direction.
        /// The length also serves as the vector magnatude.
        /// </summary>
        /// <altmember cref="LengthSquared"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude() => Magnitude(this);


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
        public float LengthSquared() => LengthSquared(this);

        /// <summary>
        /// Returns the X + Y;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Sum() => Sum(this);

        /// <summary>
        /// Returns the Abs(X) + Abs(Y);
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float SumAbs() => SumAbs(this);

        /// <summary>
        /// Calculates the euclidean distance between two points in a 2D space (slower and precisie, but not compatible with DistanceSquaredTo(...)).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistanceTo(SiVector other) => DistanceTo(this, other);

        /// <summary>
        /// Calculates the distance squared between two points in a 2D space (faster and but not compatible with DistanceTo(...)).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistanceSquaredTo(SiVector other) => DistanceSquaredTo(this, other);

        /// <summary>
        /// Calculate the angle between two points relative to the horizontal axis.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float AngleToInUnsignedRadians(SiVector point2) => AngleToInUnsignedRadians(this, point2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float AngleToInUnsignedDegrees(SiVector point2) => AngleToInUnsignedDegrees(this, point2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float AngleToInSignedRadians(SiVector point2) => AngleToInSignedRadians(this, point2);

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
        /// Determines whether the vector is normalized.
        /// </summary>
        public bool IsNormalized() => SiMath.IsOne(X * X + Y * Y);
    }
}
