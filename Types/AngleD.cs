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

        public static AngleD Degrees0 = new AngleD(0);
        public static AngleD Degrees45 = new AngleD(45.0);
        public static AngleD Degrees90 = new AngleD(90.0);
        public static AngleD Degrees180 = new AngleD(180.0);
        public static AngleD Degrees135 = new AngleD(135);
        public static AngleD Degrees225 = new AngleD(225);

        public static double RadiansToDegrees(double rad)
        {
            return rad * RAD_TO_DEG;
        }

        public static double DegreesToRadians(double deg)
        {
            return deg * DEG_TO_RAD;
        }

        #endregion

        #region ~/CTor.

        public AngleD()
        {
        }

        public AngleD(AngleD angle)
        {
            Degrees = angle.Degrees;
        }

        public AngleD(double degrees)
        {
            _degrees = degrees;
        }

        public AngleD(double x, double y)
        {
            Degrees = AngleD.RadiansToDegrees(Math.Atan2(y, x)) + DegreeOffset;
        }

        #endregion

        #region  Unary Operator Overloading.

        public static AngleD operator -(AngleD original, AngleD modifier)
        {
            return new AngleD(original.Degrees - modifier.Degrees);
        }

        public static AngleD operator -(AngleD original, double degrees)
        {
            return new AngleD(original.Degrees - degrees);
        }

        public static AngleD operator +(AngleD original, AngleD modifier)
        {
            return new AngleD(original.Degrees + modifier.Degrees);
        }

        public static AngleD operator +(AngleD original, double degrees)
        {
            return new AngleD(original.Degrees + degrees);
        }

        public static AngleD operator *(AngleD original, AngleD modifier)
        {
            return new AngleD(original.Degrees * modifier.Degrees);
        }

        public static AngleD operator *(AngleD original, double degrees)
        {
            return new AngleD(original.Degrees * degrees);
        }

        public override bool Equals(object o)
        {
            return (Math.Round(((AngleD)o).X, 4) == this.X && Math.Round(((AngleD)o).Y, 4) == this.Y);
        }

        #endregion

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round(X, 4).ToString("#.####")},{Math.Round(Y, 4).ToString("#.####")}}}";
        }

        public Vector ToVector()
        {
            return new Vector(X, Y);
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
