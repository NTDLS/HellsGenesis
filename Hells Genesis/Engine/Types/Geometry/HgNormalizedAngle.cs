using System;

namespace HG.Engine.Types.Geometry
{
    internal class HgNormalizedAngle
    {
        #region Static Utilities.

        /// <summary>
        /// Rotate the angle counter-clockwise by 90 degrees. All of our graphics math should assume this.
        /// </summary>
        public static double DegreeOffset = 90.0;
        public static double RadianOffset = Math.PI / 180 * DegreeOffset; //1.5707963267948966

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

        public static double XYToRadians(double x, double y)
        {
            return Math.Atan2(y, x) + RadianOffset;
        }

        public static double XYToDegrees(double x, double y)
        {
            return RadiansToDegrees(Math.Atan2(y, x)) + DegreeOffset;
        }

        public static HgPoint ToXY(HgNormalizedAngle angle)
        {
            return new HgPoint(angle.X, angle.Y);
        }

        public static HgPoint DegreesToXY(double degrees)
        {
            double radians = DegreesToRadians(degrees) - RadianOffset;
            return new HgPoint(Math.Cos(radians), Math.Sin(radians));
        }

        public static HgPoint RadiansToXY(double radians)
        {
            radians -= RadianOffset;
            return new HgPoint(Math.Cos(radians), Math.Sin(radians));
        }

        #endregion

        #region ~/CTor.

        public HgNormalizedAngle()
        {
        }

        public HgNormalizedAngle(HgNormalizedAngle angle)
        {
            Degrees = angle.Degrees;
        }

        public HgNormalizedAngle(double degrees)
        {
            Degrees = degrees;
        }

        public HgNormalizedAngle(double x, double y)
        {
            Degrees = RadiansToDegrees(Math.Atan2(y, x)) + DegreeOffset;
        }

        #endregion

        #region  Unary Operator Overloading.

        public static HgNormalizedAngle operator -(HgNormalizedAngle original, HgNormalizedAngle modifier)
        {
            return new HgNormalizedAngle(original.Degrees - modifier.Degrees);
        }

        public static HgNormalizedAngle operator -(HgNormalizedAngle original, double degrees)
        {
            return new HgNormalizedAngle(original.Degrees - degrees);
        }

        public static HgNormalizedAngle operator +(HgNormalizedAngle original, HgNormalizedAngle modifier)
        {
            return new HgNormalizedAngle(original.Degrees + modifier.Degrees);
        }

        public static HgNormalizedAngle operator +(HgNormalizedAngle original, double degrees)
        {
            return new HgNormalizedAngle(original.Degrees + degrees);
        }

        public static HgNormalizedAngle operator *(HgNormalizedAngle original, HgNormalizedAngle modifier)
        {
            return new HgNormalizedAngle(original.Degrees * modifier.Degrees);
        }

        public static HgNormalizedAngle operator *(HgNormalizedAngle original, double degrees)
        {
            return new HgNormalizedAngle(original.Degrees * degrees);
        }

        public override bool Equals(object o)
        {
            return Math.Round(((HgNormalizedAngle)o).X, 4) == X && Math.Round(((HgNormalizedAngle)o).Y, 4) == Y;
        }

        #endregion

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round(X, 4):#.####}x,{Math.Round(Y, 4):#.####}y}}";
        }

        public bool IsBetween(double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                return _degrees >= maxValue && _degrees <= minValue;
            }
            return _degrees >= minValue && _degrees <= maxValue;
        }

        public double _degrees;
        public double Degrees
        {
            get
            {
                return _degrees;
            }
            set
            {
                _degrees = (value + 180) % 360 - 180;
            }
        }

        public double Radians
        {
            get
            {
                return DegreesToRadians(_degrees) - RadianOffset;
            }
        }

        public double RadiansUnadjusted
        {
            get
            {
                return DegreesToRadians(_degrees);
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
