using Si.Library.Sprite;

namespace Si.Library.Mathematics.Geometry
{
    /// <summary>
    /// Static vector math helpers.
    /// </summary>
    public partial class SiPoint
    {
        public const float DEG_TO_RAD = (float)(Math.PI / 180.0);
        public const float RAD_TO_DEG = (float)(180.0 / Math.PI);
        public const float RADS_IN_CIRCLE = (float)(2 * Math.PI);

        /// <summary>
        /// 90 degrees.... but in radians.
        /// </summary>
        public const float RADIANS_90 = 90 * DEG_TO_RAD; //LEFT

        /// <summary>
        /// 270 degrees.... but in radians.
        /// </summary>
        public const float RADIANS_270 = 270 * DEG_TO_RAD; //RIGHT

        /// <summary>
        /// Rotates the given vector by the given radians.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static SiPoint Rotate(SiPoint vector, float radians)
        {
            float cosTheta = (float)Math.Cos(radians);
            float sinTheta = (float)Math.Sin(radians);
            return new SiPoint(
                vector.X * cosTheta - vector.Y * sinTheta,
                vector.X * sinTheta + vector.Y * cosTheta
            );
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="rad">Given radians to convert to degrees.</param>
        /// <returns></returns>
        public static float RadiansToDegrees(float radians) => radians * RAD_TO_DEG;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="deg">Given degrees to convert to radians.</param>
        /// <returns></returns>
        public static float DegreesToRadians(float degrees) => degrees * DEG_TO_RAD;

        /// <summary>
        /// Calculates a point at a given angle and a given distance.
        /// </summary>
        /// <param name="angle">The angle which the point should move to in the range of 0-259.</param>
        /// <param name="distance">The distance to the given angle the point should be at.</param>
        /// <returns>The calculated point at the given distance towards the given angle.</returns>
        public static SiPoint PointFromAngleAtDistance360(SiAngle angle, SiPoint distance)
            => new SiPoint((float)Math.Cos(angle.Radians) * distance.X, (float)Math.Sin(angle.Radians) * distance.Y);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        public static float AngleTo360(ISprite from, ISprite to)
            => AngleInDegreesTo360(from.Location, to.Location);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 1-180 to -1-180.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        public static float AngleTo(ISprite from, ISprite to)
        {
            var angle360 = AngleInDegreesTo360(from.Location, to.Location);
            if (angle360 > 180)
            {
                angle360 -= 180;
                angle360 = 180 - angle360;
                angle360 *= -1;
            }

            return -angle360;
        }

        /// <summary>
        /// Calculates the angle of one objects location to another location from 1-180 to -1-180.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        public static float AngleTo(ISprite from, SiPoint to)
        {
            var angle360 = AngleInDegreesTo360(from.Location, to);
            if (angle360 > 180)
            {
                angle360 -= 180;
                angle360 = 180 - angle360;
                angle360 *= -1;
            }

            return -angle360;
        }

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        public static float AngleTo360(SiPoint from, ISprite to)
            => AngleInDegreesTo360(from, to.Location);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        public static float AngleTo360(ISprite from, SiPoint to)
            => AngleInDegreesTo360(from.Location, to);

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        public static bool IsPointingAway(ISprite from, ISprite at, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle360(from, at));
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
        public static bool IsPointingAway(ISprite from, ISprite at, float toleranceDegrees, float maxDistance)
            => IsPointingAway(from, at, toleranceDegrees) && DistanceTo(from, at) <= maxDistance;

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
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
        public static bool IsPointingAt(ISprite from, ISprite at, float toleranceDegrees, float maxDistance, float offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(DeltaAngle360(from, at, offsetAngle));
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
        public static float DeltaAngle(ISprite from, ISprite to, float offsetAngle = 0)
        {
            var da360 = DeltaAngle360(from, to, offsetAngle);
            if (da360 > 180)
            {
                da360 -= 180;
                da360 = 180 - da360;
                da360 *= -1;
            }

            return -da360;
        }

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 180--180, positive figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        public static float DeltaAngle(ISprite from, SiPoint toLocation, float offsetAngle = 0)
        {
            var da360 = DeltaAngle360(from, toLocation, offsetAngle);
            if (da360 > 180)
            {
                da360 -= 180;
                da360 = 180 - da360;
                da360 *= -1;
            }

            return -da360;
        }

        /// <summary>
        /// Returns the delta angle from one object to another expressed in degrees from 0-360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand (port) side of the object, positive indicated right (starboard) side.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        public static float DeltaAngle360(ISprite from, ISprite to, float offsetAngle = 0)
        {
            float fromAngle = from.Direction.Degrees + offsetAngle;

            float angleTo = AngleTo360(from, to);

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
        public static float DeltaAngle360(ISprite from, SiPoint toLocation, float offsetAngle = 0)
        {
            float fromAngle = from.Direction.Degrees + offsetAngle;

            float angleTo = AngleTo360(from, toLocation);

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
        public static float DistanceTo(ISprite from, ISprite to)
            => DistanceTo(from.Location, to.Location);

        /// <summary>
        /// Calculates the euclidean distance between two points in a 2D space (slower and precisie, but not compatible with DistanceSquaredTo(...)).
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float DistanceTo(SiPoint from, SiPoint to)
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
        public static float DistanceSquaredTo(SiPoint from, SiPoint to)
        {
            var deltaX = to.X - from.X;
            var deltaY = to.Y - from.Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

        /// <summary>
        /// Calculates the angle from one object to another, returns the radians.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float AngleToInSignedRadians(SiPoint from, SiPoint to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        /// <summary>
        /// Calculates the angle from one object to another, returns the degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float AngleInDegreesTo360(SiPoint from, SiPoint to)
        {
            var radians = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (SiAngle.RadiansToDegrees(radians) + 360.0f) % 360.0f;
        }

        /// <summary>
        /// Normalize a vector to have a length of 1 but maintain its direction. Useful for velocity or direction vectors.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static SiPoint Normalize(SiPoint vector)
        {
            var magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            return new SiPoint(vector.X / magnitude, vector.Y / magnitude);
        }

        /// <summary>
        /// Returns the X + Y;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Sum(SiPoint a) => a.X + a.Y;

        /// <summary>
        /// Returns the Abs(X) + Abs(Y);
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float SumAbs(SiPoint a) => Math.Abs(a.X) + Math.Abs(a.Y);

        /// <summary>
        /// Calculate the dot product of two vectors.This is useful for determining the angle between vectors or projecting one vector onto another.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DotProduct(SiPoint a, SiPoint b)
            => a.X * b.X + a.Y * b.Y;

        public static float Length(SiPoint vector)
            => (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

        /// <summary>
        /// Calculate the angle between two points relative to the horizontal axis.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float AngleBetween(SiPoint point1, SiPoint point2)
        {
            return (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        }

        /// <summary>
        /// Rotate a point around another point by a certain angle.
        /// </summary>
        /// <param name="pointToRotate"></param>
        /// <param name="centerPoint"></param>
        /// <param name="angleRadians"></param>
        /// <returns></returns>
        public static SiPoint RotateAroundPoint(SiPoint pointToRotate, SiPoint centerPoint, float angleRadians)
        {
            var cosTheta = (float)Math.Cos(angleRadians);
            var sinTheta = (float)Math.Sin(angleRadians);
            var x = cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X;
            var y = sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y;
            return new SiPoint(x, y);
        }

        /// <summary>
        /// Reflect a vector off a surface.Useful for light reflections, bouncing effects, etc.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static SiPoint Reflect(SiPoint vector, SiPoint normal)
        {
            var dotProduct = DotProduct(vector, normal);
            return new SiPoint(vector.X - 2 * dotProduct * normal.X, vector.Y - 2 * dotProduct * normal.Y);
        }
    }
}
