namespace Si.Library.Types.Geometry
{
    public class SiAngle
    {
        #region Static Utilities.

        public const double DEG_TO_RAD = Math.PI / 180.0;
        public const double RAD_TO_DEG = 180.0 / Math.PI;
        public const double RADS_IN_CIRCLE = 2 * Math.PI;

        public static double RadiansToDegrees(double rad) => rad * RAD_TO_DEG;
        public static double DegreesToRadians(double deg) => deg * DEG_TO_RAD;
        public static double XYToRadians(double x, double y) => Math.Atan2(y, x);
        public static double XYToDegrees(double x, double y) => RadiansToDegrees(Math.Atan2(y, x));
        public static SiPoint ToXY(SiAngle angle) => new SiPoint(angle.X, angle.Y);
        public static SiPoint RadiansToXY(double radians) => new SiPoint(Math.Cos(radians), Math.Sin(radians));
        public static SiPoint DegreesToXY(double degrees)
        {
            double radians = DegreesToRadians(degrees);
            return new SiPoint(Math.Cos(radians), Math.Sin(radians));
        }

        #endregion

        #region ~/CTor.

        public SiAngle()
        {
        }

        public SiAngle(SiAngle angle)
        {
            Radians = angle.Radians;
        }

        public SiAngle(double radians)
        {
            Radians = radians;
        }

        public SiAngle(double x, double y)
        {
            Degrees = RadiansToDegrees(Math.Atan2(y, x));
        }

        #endregion

        #region  Unary Operator Overloading.

        public static implicit operator SiPoint(SiAngle angle)
            => new SiPoint(Math.Cos(angle.Radians), Math.Sin(angle.Radians));

        public static SiAngle operator -(SiAngle original, SiAngle modifier)
            => new SiAngle(original.Radians - modifier.Radians);

        public static SiAngle operator -(SiAngle original, double radians)
            => new SiAngle(original.Radians - radians);

        public static SiAngle operator +(SiAngle original, SiAngle modifier)
            => new SiAngle(original.Radians + modifier.Radians);

        public static SiAngle operator +(SiAngle original, double radians)
            => new SiAngle(original.Radians + radians);

        public static SiAngle operator *(SiAngle original, SiAngle scaleFactor)
            => new SiAngle(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        public static SiPoint operator *(SiAngle original, double scaleFactor)
            => new SiPoint(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiAngle operator /(SiAngle original, SiAngle scaleFactor)
            => new SiAngle(original.X / scaleFactor.X, original.Y / scaleFactor.Y);

        public static SiPoint operator /(SiAngle original, double scaleFactor)
            => new SiPoint(original.X / scaleFactor, original.Y / scaleFactor);

        public override bool Equals(object? o)
            => Math.Round(((SiAngle?)o)?.X ?? double.NaN, 4) == X && Math.Round(((SiAngle?)o)?.Y ?? double.NaN, 4) == Y;

        #endregion

        public double RadiansUnadjusted => DegreesToRadians(_degrees);
        public double X => Math.Cos(Radians);
        public double Y => Math.Sin(Radians);

        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString() => $"{{{Math.Round(X, 4):#.####}x,{Math.Round(Y, 4):#.####}y}}";

        public bool IsBetween(double minValue, double maxValue)
        {
            var normalized = DegreesNormalized180;
            if (minValue > maxValue)
            {
                return normalized >= maxValue && normalized <= minValue;
            }
            return normalized >= minValue && normalized <= maxValue;
        }

        /// <summary>
        /// Angle in degrees between 0-180 and -1--180
        /// </summary>
        public double DegreesNormalized180 => (_degrees + 180) % 360 - 180;

        /// <summary>
        /// Angle in degrees between 0-359.
        /// </summary>
        public double DegreesNormalized360 => (_degrees + 360) % 360;

        public double _degrees;
        public double Degrees
        {
            get => _degrees;

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
            get => DegreesToRadians(_degrees);
            set
            {
                if (value < 0)
                {
                    _degrees = RADS_IN_CIRCLE - Math.Abs(value) % RADS_IN_CIRCLE;
                }
                else
                {
                    _degrees = value % RADS_IN_CIRCLE;
                }

                _degrees = RadiansToDegrees(value);
            }
        }
    }
}
