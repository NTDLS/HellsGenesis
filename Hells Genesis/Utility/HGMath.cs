using HG.Actors.BaseClasses;
using HG.Types.Geometry;
using System;

namespace HG.Utility
{
    internal class HgMath
    {
        public const double AngleOffsetDegrees = 90.0;
        public const double AngleOffsetRadians = 1.5707963267948966; //(Math.PI / 180.0) * AngleOffsetDegrees;

        const double DEG_TO_RAD = Math.PI / 180.0;
        const double RAD_TO_DEG = 180.0 / Math.PI;

        public static double RadiansToDegrees(double rad)
        {
            return rad * RAD_TO_DEG;
        }

        public static double DegreesToRadians(double deg)
        {
            return deg * DEG_TO_RAD;
        }

        public static float RadiansToDegrees(float rad)
        {
            return rad * (float)RAD_TO_DEG;
        }

        public static float DegreesToRadians(float deg)
        {
            return deg * (float)DEG_TO_RAD;
        }

        /// <summary>
        /// Calculates a point at a given angle and a given distance.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static HgPoint AngleFromPointAtDistance(HgAngle angle, HgPoint distance)
        {
            return new HgPoint(
                (Math.Cos(angle.Radians) * distance.X),
                (Math.Sin(angle.Radians) * distance.Y));
        }

        /// <summary>
        /// Calculates the angle of one objects location to another location.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double AngleTo(ActorBase from, ActorBase to)
        {
            return HgPoint.AngleTo(from.Location, to.Location);
        }

        public static double AngleTo(HgPoint from, ActorBase to)
        {
            return HgPoint.AngleTo(from, to.Location);
        }

        public static double AngleTo(ActorBase from, HgPoint to)
        {
            return HgPoint.AngleTo(from.Location, to);
        }

        public static bool IsPointingAway(ActorBase fromObj, ActorBase atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle360(fromObj, atObj));
            return deltaAngle < 180 + toleranceDegrees && deltaAngle > 180 - toleranceDegrees;
        }

        public static bool IsPointingAway(ActorBase fromObj, ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return IsPointingAway(fromObj, atObj, toleranceDegrees) && DistanceTo(fromObj, atObj) <= maxDistance;
        }

        public static bool IsPointingAt(ActorBase fromObj, ActorBase atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle(fromObj, atObj));
            return deltaAngle < toleranceDegrees || deltaAngle > (360 - toleranceDegrees);
        }

        public static bool IsPointingAt(ActorBase fromObj, ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(DeltaAngle360(fromObj, atObj, offsetAngle));
            if (deltaAngle < toleranceDegrees || deltaAngle > (360 - toleranceDegrees))
            {
                return DistanceTo(fromObj, atObj) <= maxDistance;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromObj"></param>
        /// <param name="toObj"></param>
        /// <param name="offsetAngle">-90 degrees would be looking off the left-hand side of the object</param>
        /// <returns></returns>
        public static double DeltaAngle(ActorBase fromObj, ActorBase toObj, double offsetAngle = 0)
        {
            var da360 = DeltaAngle360(fromObj, toObj, offsetAngle);
            if (da360 > 180)
            {
                da360 -= 180;
                da360 = 180 - da360;
                da360 *= -1;
            }

            return -da360;
        }

        public static double DeltaAngle(ActorBase fromObj, HgPoint toLocation, double offsetAngle = 0)
        {
            var da360 = DeltaAngle360(fromObj, toLocation, offsetAngle);
            if (da360 > 180)
            {
                da360 -= 180;
                da360 = 180 - da360;
                da360 *= -1;
            }

            return -da360;
        }

        public static double DeltaAngle360(ActorBase fromObj, ActorBase toObj, double offsetAngle = 0)
        {
            double fromAngle = fromObj.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo(fromObj, toObj);

            if (fromAngle < 0) fromAngle = (0 - fromAngle);
            if (angleTo < 0)
            {
                angleTo = (0 - angleTo);
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0 - (Math.Abs(angleTo) % 360.0);
            }

            return angleTo;
        }

        public static double DeltaAngle360(ActorBase fromObj, HgPoint toLocation, double offsetAngle = 0)
        {
            double fromAngle = fromObj.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo(fromObj, toLocation);

            if (fromAngle < 0) fromAngle = (0 - fromAngle);
            if (angleTo < 0)
            {
                angleTo = (0 - angleTo);
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0 - (Math.Abs(angleTo) % 360.0);
            }

            return angleTo;
        }

        public static double DistanceTo(ActorBase from, ActorBase to)
        {
            return HgPoint.DistanceTo(from.Location, to.Location);
        }
    }
}
