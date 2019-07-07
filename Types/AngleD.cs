using System;
using System.Windows;

namespace AI2D.Types
{
    public class AngleD
    {
        #region Static Utilities.

        /// <summary>
        /// Rotate the angle counter-clockwise by 90 degrees. All of our graphics math should assume this.
        /// </summary>
        public static double DegreeOffset = 90.0;
        public static double RadianOffset = (Math.PI / 180) * DegreeOffset; //1.5707963267948966

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

        public static double DistanceTo(PointD from, PointD to)
        {
            var deltaX = Math.Pow((to.X - from.X), 2);
            var deltaY = Math.Pow((to.Y - from.Y), 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        public static double AngleTo(PointD from, PointD to)
        {
            var fRadians = Math.Atan2((to.Y - from.Y), (to.X - from.X));
            var fDegrees = ((RadiansToDegrees(fRadians) + 360.0) + DegreeOffset) % 360.0;
            return fDegrees;
        }

        #endregion

        public AngleD()
        {
        }

        public AngleD(double degrees)
        {
            _degrees = degrees;
        }

        public AngleD(double x, double y)
        {
            Degrees = AngleD.RadiansToDegrees(Math.Atan2(y, x)) + DegreeOffset;
        }

        public Vector Vector
        {
            get
            {
                return new Vector(X, Y);
            }
        }

        public double _degrees = 0;
        public double Degrees
        {
            get
            {
                return _degrees;
            }
            set
            {
                _degrees = (value % 360.0);
            }
        }

        public double Radians
        {
            get
            {
                return AngleD.DegreesToRadians(_degrees) - RadianOffset;
            }
        }

        public double RadiansUnadjusted
        {
            get
            {
                return AngleD.DegreesToRadians(_degrees);
            }
        }

        public double X
        {
            get
            {
                return Math.Cos(Radians);
            }
        }

        public double Y
        {
            get
            {
                return Math.Sin(Radians);
            }
        }
    }
}
