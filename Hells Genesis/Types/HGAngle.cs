using System;

namespace HG.Types
{
    internal class HgAngle<T>
    {
        #region Static Utilities.

        /// <summary>
        /// Rotate the angle counter-clockwise by 90 degrees. All of our graphics math should assume this.
        /// </summary>
        public static double DegreeOffset = 90.0;
        public static double RadianOffset = (Math.PI / 180) * DegreeOffset; //1.5707963267948966

        const double DEG_TO_RAD = Math.PI / 180.0;
        const double RAD_TO_DEG = 180.0 / Math.PI;

        public static double RadiansToDegrees(T rad)
        {
            return rad * (dynamic)RAD_TO_DEG;
        }

        public static T DegreesToRadians(T deg)
        {
            return deg * (dynamic)DEG_TO_RAD;
        }

        public static T XYToRadians(T x, T y)
        {
            return Math.Atan2((dynamic)y, (dynamic)x) + RadianOffset;
        }

        public static T XYToDegrees(T x, T y)
        {
            return RadiansToDegrees(Math.Atan2((dynamic)y, (dynamic)x)) + DegreeOffset;
        }

        public static HgPoint<T> ToXY(HgAngle<T> angle)
        {
            return new HgPoint<T>(angle.X, angle.Y);
        }

        public static HgPoint<T> DegreesToXY(T degrees)
        {
            T radians = DegreesToRadians(degrees) - (dynamic)RadianOffset;
            return new HgPoint<T>(Math.Cos((dynamic)radians), Math.Sin((dynamic)radians));
        }

        public static HgPoint<T> RadiansToXY(T radians)
        {
            radians -= (dynamic)RadianOffset;
            return new HgPoint<T>(Math.Cos((dynamic)radians), Math.Sin((dynamic)radians));
        }

        #endregion

        #region ~/CTor.

        public HgAngle()
        {
        }

        public HgAngle(HgAngle<T> angle)
        {
            Degrees = angle.Degrees;
        }

        public HgAngle(T degrees)
        {
            Degrees = degrees;
        }

        public HgAngle(T x, T y)
        {
            Degrees = RadiansToDegrees(Math.Atan2((dynamic)y, (dynamic)x)) + DegreeOffset;
        }

        #endregion

        #region  Unary Operator Overloading.

        public static HgAngle<T> operator -(HgAngle<T> original, HgAngle<T> modifier)
        {
            return new HgAngle<T>(original.Degrees - (dynamic)modifier.Degrees);
        }

        public static HgAngle<T> operator -(HgAngle<T> original, T degrees)
        {
            return new HgAngle<T>(original.Degrees - (dynamic)degrees);
        }

        public static HgAngle<T> operator +(HgAngle<T> original, HgAngle<T> modifier)
        {
            return new HgAngle<T>(original.Degrees + (dynamic)modifier.Degrees);
        }

        public static HgAngle<T> operator +(HgAngle<T> original, T degrees)
        {
            return new HgAngle<T>(original.Degrees + (dynamic)degrees);
        }

        public static HgAngle<T> operator *(HgAngle<T> original, HgAngle<T> modifier)
        {
            return new HgAngle<T>(original.Degrees * (dynamic)modifier.Degrees);
        }

        public static HgAngle<T> operator *(HgAngle<T> original, T degrees)
        {
            return new HgAngle<T>(original.Degrees * (dynamic)degrees);
        }

        public override bool Equals(object o)
        {
            return (Math.Round((dynamic)((HgAngle<T>)o).X, 4) == this.X && Math.Round((dynamic)((HgAngle<T>)o).Y, 4) == this.Y);
        }

        #endregion

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round((dynamic)X, 4):#.####}x,{Math.Round((dynamic)Y, 4):#.####}y}}";
        }


        public T _degrees;
        public T Degrees
        {
            get
            {
                return _degrees;
            }
            set
            {
                if ((dynamic)value < 0)
                {
                    _degrees = (360 - (Math.Abs((dynamic)value) % 360.0));
                }
                else
                {
                    _degrees = ((dynamic)value) % 360;
                }
            }
        }

        public T Radians
        {
            get
            {
                return DegreesToRadians(_degrees) - (dynamic)RadianOffset;
            }
        }

        public T RadiansUnadjusted
        {
            get
            {
                return DegreesToRadians(_degrees);
            }
        }

        public T X
        {
            get
            {
                return Math.Cos((dynamic)Radians);
            }
        }

        public T Y
        {
            get
            {
                return Math.Sin((dynamic)Radians);
            }
        }
    }
}
