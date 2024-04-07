using System.Runtime.CompilerServices;

namespace Si.Library.Mathematics.Geometry
{
    /// <summary>
    /// An angle, which is a normalized vector. Degrees, Radians and Cartesian Coordinates.
    /// </summary>
    public class SiAngle
    {
        public const float DEG_TO_RAD = (float)(Math.PI / 180.0);
        public const float RAD_TO_DEG = (float)(180.0 / Math.PI);
        public const float RADS_IN_CIRCLE = (float)(2 * Math.PI);

        /// <summary>
        /// 90 (looking right) degrees.... but in radians.
        /// </summary>
        public const float RADIANS_90 = 90 * DEG_TO_RAD;

        /// <summary>
        /// 270 degrees (looking left) .... but in radians.
        /// </summary>
        public const float RADIANS_270 = 270 * DEG_TO_RAD;

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Rad2Deg(float radians) => radians * RAD_TO_DEG;

        /// <summary>
        /// Converts an x,y to degrees.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float XyToDeg(float x, float y) => Rad2Deg((float)Math.Atan2(y, x));

        /// <summary>
        /// COnverts Degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Deg2Rad(float degrees) => degrees * SiVector.DEG_TO_RAD;

        /// <summary>
        /// Converts x,y to radians.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float XyToRad(float x, float y) => (float)Math.Atan2(y, x);

        /// <summary>
        /// Converts radians to a normalized vector.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector RadToVector(float radians) => new((float)Math.Cos(radians), (float)Math.Sin(radians));

        /// <summary>
        /// Converts degrees to a normalized vector.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector DegToVector(float degrees) => new((float)Math.Cos(degrees * RAD_TO_DEG), (float)Math.Sin(degrees * RAD_TO_DEG));

        /// <summary>
        /// Returns an SiAngle from degrees.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle FromDeg(float degrees) => new SiAngle(Deg2Rad(degrees));

        /// <summary>
        /// Returns an SiAngle from radians.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle FromRad(float radians) => new SiAngle(radians);

        /// <summary>
        /// Returns an SiAngle from a SiVector (vector will be normailzed before conversion).
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle FromXy(SiVector vector) => new SiAngle(vector);

        /// <summary>
        /// Returns a normailized vector from an SiAngle.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector ToVector(SiAngle angle) => new(angle.X, angle.Y);

        #region ~/CTor.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiAngle() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiAngle(SiAngle angle) => Radians = angle.Radians;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiAngle(float radians) => Radians = radians;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SiAngle(float x, float y) => Degrees = Rad2Deg((float)Math.Atan2(y, x));

        public SiAngle(SiVector vector)
        {
            var directionVector = SiVector.Normalize(vector);
            Degrees = Rad2Deg((float)Math.Atan2(directionVector.Y, directionVector.X));
        }

        #endregion

        #region  Unary Operator Overloading.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SiVector(SiAngle angle)
            => new SiVector((float)Math.Cos(angle.Radians), (float)Math.Sin(angle.Radians));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle operator -(SiAngle original, SiAngle modifier)
            => new SiAngle(original.Radians - modifier.Radians);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle operator -(SiAngle original, float radians)
            => new SiAngle(original.Radians - radians);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle operator +(SiAngle original, SiAngle modifier)
            => new SiAngle(original.Radians + modifier.Radians);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle operator +(SiAngle original, float radians)
            => new SiAngle(original.Radians + radians);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle operator *(SiAngle original, SiAngle scaleFactor)
            => new SiAngle(original.X * scaleFactor.X, original.Y * scaleFactor.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator *(SiAngle original, float scaleFactor)
            => new SiVector(original.X * scaleFactor, original.Y * scaleFactor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiAngle operator /(SiAngle original, SiAngle scaleFactor)
            => new SiAngle(original.X / scaleFactor.X, original.Y / scaleFactor.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SiVector operator /(SiAngle original, float scaleFactor)
            => new SiVector(original.X / scaleFactor, original.Y / scaleFactor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            get => Deg2Rad(_degrees);
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

                _degrees = Rad2Deg(value);
            }
        }
    }
}
