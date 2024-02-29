namespace Si.Library.Mathematics.Geometry
{
    /// <summary>
    /// Normalized angles. Degrees, Radians and Cartesian Coordinates.
    /// </summary>
    public class SiAngle
    {
        public const double DEG_TO_RAD = Math.PI / 180.0;
        public const double RAD_TO_DEG = 180.0 / Math.PI;
        public const double RADS_IN_CIRCLE = 2 * Math.PI;

        public static double RadiansToDegrees(double rad) => rad * RAD_TO_DEG;
        public static double DegreesToRadians(double deg) => deg * DEG_TO_RAD;
        public static double XYToRadians(double x, double y) => Math.Atan2(y, x);
        public static double XYToDegrees(double x, double y) => RadiansToDegrees(Math.Atan2(y, x));
        public static SiVector ToXY(SiAngle angle) => new(angle.X, angle.Y);
        public static SiVector RadiansToXY(double radians) => new(Math.Cos(radians), Math.Sin(radians));
        public static SiVector DegreesToXY(double degrees) => new(Math.Cos(degrees * RAD_TO_DEG), Math.Sin(degrees * RAD_TO_DEG));

        #region ~/CTor.

        public SiAngle() { }
        public SiAngle(SiAngle angle) => Radians = angle.Radians;
        public SiAngle(double radians) => Radians = radians;
        public SiAngle(double x, double y) => Degrees = RadiansToDegrees(Math.Atan2(y, x));

        #endregion

        #region  Unary Operator Overloading.

        public static implicit operator SiVector(SiAngle angle)
            => new SiVector(Math.Cos(angle.Radians), Math.Sin(angle.Radians));

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

        public static SiVector operator *(SiAngle original, double scaleFactor)
            => new SiVector(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiAngle operator /(SiAngle original, SiAngle scaleFactor)
            => new SiAngle(original.X / scaleFactor.X, original.Y / scaleFactor.Y);

        public static SiVector operator /(SiAngle original, double scaleFactor)
            => new SiVector(original.X / scaleFactor, original.Y / scaleFactor);

        public override bool Equals(object? o)
            => Math.Round(((SiAngle?)o)?.X ?? double.NaN, 4) == X && Math.Round(((SiAngle?)o)?.Y ?? double.NaN, 4) == Y;

        #endregion

        /// <summary>
        /// Cartesian coordinate X.
        /// </summary>
        public double X => Math.Cos(Radians);

        /// <summary>
        /// Cartesian coordinate Y.
        /// </summary>
        public double Y => Math.Sin(Radians);

        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString() => $"{{{Math.Round(X, 4):#.####}x,{Math.Round(Y, 4):#.####}y}}";

        /// <summary>
        /// Returns true if the current angle is between the given values.
        /// </summary>
        /// <param name="minSignedDegrees"></param>
        /// <param name="maxSignedDegrees"></param>
        /// <returns></returns>
        public bool IsBetween(double minSignedDegrees, double maxSignedDegrees)
        {
            var signedDegrees = DegreesSigned;
            if (minSignedDegrees > maxSignedDegrees)
            {
                return signedDegrees >= maxSignedDegrees && signedDegrees <= minSignedDegrees;
            }
            return signedDegrees >= minSignedDegrees && signedDegrees <= maxSignedDegrees;
        }

        /// <summary>
        /// Angle in degrees between [−180,180]
        /// </summary>
        public double DegreesSigned => (_degrees + 180) % 360 - 180;

        /// <summary>
        /// Angle in radians between [−3.14,3.14]
        /// </summary>
        public double RadiansSigned => (Radians + Math.PI) % (Math.PI * 2) - Math.PI;

        public double _degrees;
        /// <summary>
        /// Sets the angle degrees and ensures that it is stored denoramlized (0-359).
        /// </summary>
        public double Degrees
        {
            get => _degrees;

            set
            {
                if (value < 0)
                {
                    _degrees = (value + 360) % 360;
                }
                else
                {
                    _degrees = value % 360;
                }
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
