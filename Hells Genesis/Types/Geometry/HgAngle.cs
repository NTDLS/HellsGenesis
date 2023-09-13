using System;

namespace HG.Types.Geometry
{
    internal class HgAngle
    {
        #region Static Utilities.

        /// <summary>
        /// Rotate the angle counter-clockwise by 90 degrees. All of our graphics math should assume this.
        /// </summary>
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

        public static double XYToRadians(double x, double y)
        {
            return Math.Atan2(y, x) + AngleOffsetRadians;
        }

        public static double XYToDegrees(double x, double y)
        {
            return RadiansToDegrees(Math.Atan2(y, x)) + AngleOffsetDegrees;
        }

        public static HgPoint ToXY(HgAngle angle)
        {
            return new HgPoint(angle.X, angle.Y);
        }

        public static HgPoint DegreesToXY(double degrees)
        {
            double radians = DegreesToRadians(degrees) - AngleOffsetRadians;
            return new HgPoint(Math.Cos(radians), Math.Sin(radians));
        }

        public static HgPoint RadiansToXY(double radians)
        {
            radians -= AngleOffsetRadians;
            return new HgPoint(Math.Cos(radians), Math.Sin(radians));
        }

        #endregion

        #region ~/CTor.

        public HgAngle()
        {
        }

        public HgAngle(HgAngle angle)
        {
            Degrees = angle.Degrees;
        }

        public HgAngle(double degrees)
        {
            Degrees = degrees;
        }

        public HgAngle(double x, double y)
        {
            Degrees = RadiansToDegrees(Math.Atan2(y, x)) + AngleOffsetDegrees;
        }

        #endregion

        #region  Unary Operator Overloading.

        public static HgAngle operator -(HgAngle original, HgAngle modifier)
        {
            return new HgAngle(original.Degrees - modifier.Degrees);
        }

        public static HgAngle operator -(HgAngle original, double degrees)
        {
            return new HgAngle(original.Degrees - degrees);
        }

        public static HgAngle operator +(HgAngle original, HgAngle modifier)
        {
            return new HgAngle(original.Degrees + modifier.Degrees);
        }

        public static HgAngle operator +(HgAngle original, double degrees)
        {
            return new HgAngle(original.Degrees + degrees);
        }

        public static HgAngle operator *(HgAngle original, HgAngle modifier)
        {
            return new HgAngle(original.Degrees * modifier.Degrees);
        }

        public static HgAngle operator *(HgAngle original, double degrees)
        {
            return new HgAngle(original.Degrees * degrees);
        }

        public override bool Equals(object o)
        {
            return Math.Round(((HgAngle)o).X, 4) == X && Math.Round(((HgAngle)o).Y, 4) == Y;
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
            var normalized = DegreesNormalized;
            if (minValue > maxValue)
            {
                return normalized >= maxValue && normalized <= minValue;
            }
            return normalized >= minValue && normalized <= maxValue;
        }

        public double DegreesNormalized
        {
            get
            {
                return (_degrees + 180) % 360 - 180;
            }
        }

        public double DegreesNormalized360
        {
            get
            {
                return (_degrees + 360) % 360;
            }
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
                if (value < 0)
                {
                    _degrees = 360 - Math.Abs(value) % 360.0;
                }
                else
                {
                    _degrees = value % 360;
                }
            }
        }

        public double Radians
        {
            get
            {
                return DegreesToRadians(_degrees) - AngleOffsetRadians;
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
