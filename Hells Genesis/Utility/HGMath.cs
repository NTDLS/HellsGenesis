using HG.Actors;
using HG.Types;
using Newtonsoft.Json.Linq;
using System;

namespace HG.Engine
{
    internal class HgMath
    {
        /// <summary>
        /// Calculates a point at a given angle and a given distance.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static HgPoint<double> AngleFromPointAtDistance(HgAngle<double> angle, HgPoint<double> distance)
        {
            return new HgPoint<double>(
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
            return HgPoint<double>.AngleTo(from.Location, to.Location);
        }

        public static double AngleTo(HgPoint<double> from, ActorBase to)
        {
            return HgPoint<double>.AngleTo(from, to.Location);
        }

        public static double AngleTo(ActorBase from, HgPoint<double> to)
        {
            return HgPoint<double>.AngleTo(from.Location, to);
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
        /// <param name="atObj"></param>
        /// <param name="offsetAngle">-90 degrees would be looking off te left-hand side of the object</param>
        /// <returns></returns>
        public static double DeltaAngle(ActorBase fromObj, ActorBase atObj, double offsetAngle = 0)
        {
            double fromAngle = fromObj.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo(fromObj, atObj);

            if (fromAngle < 0) fromAngle = (0 - fromAngle);
            if (angleTo < 0)
            {
                angleTo = (0 - angleTo);
            }

            return (fromAngle - angleTo + 180) % 360 - 180;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromObj"></param>
        /// <param name="atObj"></param>
        /// <param name="offsetAngle">-90 degrees would be looking off te left-hand side of the object</param>
        /// <returns></returns>
        public static double DeltaAngle360(ActorBase fromObj, ActorBase atObj, double offsetAngle = 0)
        {
            double fromAngle = fromObj.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo(fromObj, atObj);

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
            return HgPoint<double>.DistanceTo(from.Location, to.Location);
        }
    }
}
