using Si.GameEngine.Sprites._Superclass;
using Si.Library.Types.Geometry;
using System;

namespace Si.GameEngine.Utility
{
    internal class SiSpriteVectorMath
    {
        public const double DEG_TO_RAD = Math.PI / 180.0;
        public const double RAD_TO_DEG = 180.0 / Math.PI;
        public const double DEG_90_RADS = 90 * DEG_TO_RAD; //LEFT
        public const double DEG_270_RADS = 270 * DEG_TO_RAD; //RIGHT

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="rad">Given radians to convert to degrees.</param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians) => radians * RAD_TO_DEG;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="deg">Given degrees to convert to radians.</param>
        /// <returns></returns>
        public static double DegreesToRadians(double degrees) => degrees * DEG_TO_RAD;


        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="rad">Given radians to convert to degrees.</param>
        /// <returns></returns>
        public static float RadiansToDegrees(float radians) => radians * (float)RAD_TO_DEG;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="deg">Given degrees to convert to radians.</param>
        /// <returns></returns>
        public static float DegreesToRadians(float degrees) => degrees * (float)DEG_TO_RAD;

        /// <summary>
        /// Calculates a point at a given angle and a given distance.
        /// </summary>
        /// <param name="angle">The angle which the point should move to in the range of 0-259.</param>
        /// <param name="distance">The distance to the given angle the point should be at.</param>
        /// <returns>The calculated point at the given distance towards the given angle.</returns>
        public static SiPoint PointFromAngleAtDistance360(SiAngle angle, SiPoint distance)
            => new SiPoint(Math.Cos(angle.Radians) * distance.X, Math.Sin(angle.Radians) * distance.Y);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        public static double AngleTo360(SpriteBase from, SpriteBase to)
            => SiPoint.AngleTo360(from.Location, to.Location);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 1-180 to -1-180.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        public static double AngleTo(SpriteBase from, SpriteBase to)
        {
            var angle360 = SiPoint.AngleTo360(from.Location, to.Location);
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
        public static double AngleTo(SpriteBase from, SiPoint to)
        {
            var angle360 = SiPoint.AngleTo360(from.Location, to);
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
        public static double AngleTo360(SiPoint from, SpriteBase to)
            => SiPoint.AngleTo360(from, to.Location);

        /// <summary>
        /// Calculates the angle of one objects location to another location from 0 - 360.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        public static double AngleTo360(SpriteBase from, SiPoint to)
            => SiPoint.AngleTo360(from.Location, to);

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        public static bool IsPointingAway(SpriteBase from, SpriteBase at, double toleranceDegrees)
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
        public static bool IsPointingAway(SpriteBase from, SpriteBase at, double toleranceDegrees, double maxDistance)
            => IsPointingAway(from, at, toleranceDegrees) && DistanceTo(from, at) <= maxDistance;

        /// <summary>
        /// Returns true if the object is pointing AT another, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="at">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        public static bool IsPointingAt(SpriteBase from, SpriteBase at, double toleranceDegrees)
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
        public static bool IsPointingAt(SpriteBase from, SpriteBase at, double toleranceDegrees, double maxDistance, double offsetAngle = 0)
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
        public static double DeltaAngle(SpriteBase from, SpriteBase to, double offsetAngle = 0)
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
        public static double DeltaAngle(SpriteBase from, SiPoint toLocation, double offsetAngle = 0)
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
        public static double DeltaAngle360(SpriteBase from, SpriteBase to, double offsetAngle = 0)
        {
            double fromAngle = from.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo360(from, to);

            if (fromAngle < 0) fromAngle = 0 - fromAngle;
            if (angleTo < 0)
            {
                angleTo = 0 - angleTo;
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0 - Math.Abs(angleTo) % 360.0;
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
        public static double DeltaAngle360(SpriteBase from, SiPoint toLocation, double offsetAngle = 0)
        {
            double fromAngle = from.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo360(from, toLocation);

            if (fromAngle < 0) fromAngle = 0 - fromAngle;
            if (angleTo < 0)
            {
                angleTo = 0 - angleTo;
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0 - Math.Abs(angleTo) % 360.0;
            }

            return angleTo;
        }


        /// <summary>
        /// Returns the distance from one object to another.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The object to which the calculation is based.</param>
        /// <returns>The calcuated distance from one object to the other.</returns>
        public static double DistanceTo(SpriteBase from, SpriteBase to)
            => SiPoint.DistanceTo(from.Location, to.Location);
    }
}
