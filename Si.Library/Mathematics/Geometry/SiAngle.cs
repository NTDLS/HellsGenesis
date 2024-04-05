namespace Si.Library.Mathematics.Geometry
{
    /// <summary>
    /// Normalized angles. Degrees, Radians and Cartesian Coordinates.
    /// </summary>
    public class SiAngle
    {
        public const float DEG_TO_RAD = (float)(Math.PI / 180.0);
        public const float RAD_TO_DEG = (float)(180.0 / Math.PI);
        public const float RADS_IN_CIRCLE = (float)(2 * Math.PI);

        public static float RadiansToDegrees(float rad) => rad * RAD_TO_DEG;
        public static float DegreesToRadians(float deg) => deg * DEG_TO_RAD;
        public static float XYToRadians(float x, float y) => (float)Math.Atan2(y, x);
        public static float XYToDegrees(float x, float y) => RadiansToDegrees((float)Math.Atan2(y, x));

        public static SiVector ToXY(SiAngle angle) => new(angle.X, angle.Y);
        public static SiVector RadiansToXY(float radians) => new((float)Math.Cos(radians), (float)Math.Sin(radians));
        public static SiVector DegreesToXY(float degrees) => new((float)Math.Cos(degrees * RAD_TO_DEG), (float)Math.Sin(degrees * RAD_TO_DEG));
        public static SiAngle FromXY(SiVector point) => new SiAngle(XYToRadians(point.X, point.Y));

        #region ~/CTor.

        public SiAngle() { }
        public SiAngle(SiAngle angle) => Radians = angle.Radians;
        public SiAngle(float radians) => Radians = radians;
        public SiAngle(float x, float y) => Degrees = RadiansToDegrees((float)Math.Atan2(y, x));

        #endregion

        #region  Unary Operator Overloading.

        public static implicit operator SiVector(SiAngle angle)
            => new SiVector((float)Math.Cos(angle.Radians), (float)Math.Sin(angle.Radians));

        public static SiAngle operator -(SiAngle original, SiAngle modifier)
            => new SiAngle(original.Radians - modifier.Radians);

        public static SiAngle operator -(SiAngle original, float radians)
            => new SiAngle(original.Radians - radians);

        public static SiAngle operator +(SiAngle original, SiAngle modifier)
            => new SiAngle(original.Radians + modifier.Radians);

        public static SiAngle operator +(SiAngle original, float radians)
            => new SiAngle(original.Radians + radians);

        public static SiAngle operator *(SiAngle original, SiAngle scaleFactor)
            => new SiAngle(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        public static SiVector operator *(SiAngle original, float scaleFactor)
            => new SiVector(original.X * scaleFactor, original.Y * scaleFactor);

        public static SiAngle operator /(SiAngle original, SiAngle scaleFactor)
            => new SiAngle(original.X / scaleFactor.X, original.Y / scaleFactor.Y);

        public static SiVector operator /(SiAngle original, float scaleFactor)
            => new SiVector(original.X / scaleFactor, original.Y / scaleFactor);

        public override bool Equals(object? o)
            => Math.Round(((SiAngle?)o)?.X ?? float.NaN, 4) == X && Math.Round(((SiAngle?)o)?.Y ?? float.NaN, 4) == Y;

        #endregion

        /// <summary>
        /// Cartesian coordinate X.
        /// </summary>
        public float X => (float)Math.Cos(Radians);

        /// <summary>
        /// Cartesian coordinate Y.
        /// </summary>
        public float Y => (float)Math.Sin(Radians);

        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString() => $"{{{Math.Round(X, 4):#.####}x,{Math.Round(Y, 4):#.####}y}}";

        /// <summary>
        /// Returns true if the current angle is between the given values.
        /// </summary>
        /// <param name="minSignedDegrees"></param>
        /// <param name="maxSignedDegrees"></param>
        /// <returns></returns>
        public bool IsBetween(float minSignedDegrees, float maxSignedDegrees)
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
        public float DegreesSigned => (_degrees + 180) % 360 - 180;

        /// <summary>
        /// Angle in radians between [−3.14,3.14]
        /// </summary>
        public float RadiansSigned => (float)((Radians + Math.PI) % (Math.PI * 2) - Math.PI);

        public float _degrees;
        /// <summary>
        /// Sets the angle degrees and ensures that it is stored denoramlized (0-359).
        /// </summary>
        public float Degrees
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
        public float Radians
        {
            get => DegreesToRadians(_degrees);
            set
            {
                if (value < 0)
                {
                    value = (value + RADS_IN_CIRCLE) % RADS_IN_CIRCLE;
                }
                else
                {
                    value %= RADS_IN_CIRCLE;
                }

                _degrees = RadiansToDegrees(value);
            }
        }
    }
}
