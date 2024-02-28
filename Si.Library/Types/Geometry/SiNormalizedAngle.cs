namespace Si.Library.Types.Geometry
{
    public class SiNormalizedAngle
    {
        #region Static Utilities.

        public const double DEG_TO_RAD = Math.PI / 180.0;
        public const double RAD_TO_DEG = 180.0 / Math.PI;
        public const double RADS_IN_CIRCLE = 2 * Math.PI;

        public static double RadiansToDegrees(double rad) => rad * RAD_TO_DEG;
        public static double DegreesToRadians(double deg) => deg * DEG_TO_RAD;
        public static double XYToRadians(double x, double y) => Math.Atan2(y, x);
        public static double XYToDegrees(double x, double y) => RadiansToDegrees(Math.Atan2(y, x));
        public static SiPoint ToXY(SiNormalizedAngle angle) => new(angle.X, angle.Y);
        public static SiPoint RadiansToXY(double radians) => new(Math.Cos(radians), Math.Sin(radians));
        public static SiPoint DegreesToXY(double degrees) => new(Math.Cos(degrees * RAD_TO_DEG), Math.Sin(degrees * RAD_TO_DEG));

        #endregion

        #region ~/CTor.

        public SiNormalizedAngle()
        {
        }

        public SiNormalizedAngle(SiNormalizedAngle angle)
        {
            Degrees = angle.Degrees;
        }

        public SiNormalizedAngle(double degrees)
        {
            Degrees = degrees;
        }

        public SiNormalizedAngle(double x, double y)
        {
            Degrees = RadiansToDegrees(Math.Atan2(y, x));
        }

        #endregion

        #region  Unary Operator Overloading.

        public static implicit operator SiPoint(SiNormalizedAngle angle)
            => new SiPoint(Math.Cos(angle.Radians), Math.Sin(angle.Radians));

        public static SiNormalizedAngle operator -(SiNormalizedAngle original, SiNormalizedAngle modifier)
            => new SiNormalizedAngle(original.Radians - modifier.Radians);

        public static SiNormalizedAngle operator -(SiNormalizedAngle original, double radians)
            => new SiNormalizedAngle(original.Radians - radians);

        public static SiNormalizedAngle operator +(SiNormalizedAngle original, SiNormalizedAngle modifier)
            => new SiNormalizedAngle(original.Radians + modifier.Radians);

        public static SiNormalizedAngle operator +(SiNormalizedAngle original, double radians)
            => new SiNormalizedAngle(original.Radians + radians);

        public static SiNormalizedAngle operator *(SiNormalizedAngle original, SiNormalizedAngle scaleFactor)
            => new SiNormalizedAngle(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        public static SiPoint operator *(SiNormalizedAngle original, double scaleFactor)
            => new SiPoint(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiNormalizedAngle operator /(SiNormalizedAngle original, SiNormalizedAngle scaleFactor)
            => new SiNormalizedAngle(original.X / scaleFactor.X, original.Y / scaleFactor.Y);

        public static SiPoint operator /(SiNormalizedAngle original, double scaleFactor)
            => new SiPoint(original.X / scaleFactor, original.Y / scaleFactor);

        public override bool Equals(object? o)
            => Math.Round(((SiNormalizedAngle?)o)?.X ?? double.NaN, 4) == X && Math.Round(((SiNormalizedAngle?)o)?.Y ?? double.NaN, 4) == Y;

        #endregion

        public double RadiansUnadjusted => DegreesToRadians(_degrees);
        public double X => Math.Cos(Radians);
        public double Y => Math.Sin(Radians);

        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString() => $"{{{Math.Round(X, 4):#.####}x,{Math.Round(Y, 4):#.####}y}}";

        public bool IsBetween(double minValue, double maxValue)
        {
            if (minValue > maxValue)
            {
                return _degrees >= maxValue && _degrees <= minValue;
            }
            return _degrees >= minValue && _degrees <= maxValue;
        }

        public double _degrees;

        /// <summary>
        /// Sets the angle degrees and ensures that it is stored noramlized (0 thorugh 180 and -0 thorugh -180).
        /// </summary>
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

        /// <summary>
        /// Sets the angle radians and ensures that it is stored denoramlized (0-6.28).
        /// </summary>
        public double Radians
        {
            get => DegreesToRadians(_degrees);
            set
            {
                if (value < 0)
                {
                    _degrees = (value + RADS_IN_CIRCLE) % RADS_IN_CIRCLE;
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
