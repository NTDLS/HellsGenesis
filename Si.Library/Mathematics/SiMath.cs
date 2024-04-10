using System.Runtime.CompilerServices;

namespace Si.Library.Mathematics
{
    public static class SiMath
    {
        /* Notes, because I have the memory of a goldfish:
         * 
         *  Revolution
         *      A "revolution" measures a complete turn around a circle. It is equivalent to 360 degrees, 
         *      2π radians, or 400 gradians. Using revolutions can simplify some calculations, especially
         *      when working with entities that rotate multiple times, since it directly represents whole
         *      circle rotations. The conversion functions provided in your code snippet are used to convert
         *      between revolutions and other units of angle measurement (degrees, radians, and gradians).
         *
         *  Gradient (Gradian)
         *       A "gradian" (also known as "gon") is another unit of angle measurement. There are 400 gradians
         *       in a full circle, making it different from degrees and radians. One gradian equals 1/400 of a full
         *       circle, so it represents a slightly smaller angle unit than a degree (1 degree = 1/360 of a full circle).
         *       The function RevolutionsToGradians converts the number of full circle revolutions into gradians.
         *
         ***********************************************************************************************************************
         *   Application in Physics Simulations:
         ***********************************************************************************************************************
         *   Revolutions:
         *      Often used for simplifying the concept of rotations in animations, physics simulations,
         *       or when applying transformations to physics objects. For instance, specifying that an object should
         *       rotate twice fully can be simply stated as 2 revolutions.
         *
         *   Radians:
         *      Preferred in mathematics and physics calculations because they can make certain calculations,
         *      especially those involving trigonometry, more straightforward.
         *
         *   Degrees:
         *      Commonly used due to their intuitive understanding in everyday language and simplicity in
         *      specifying angles less than a full circle.
         *
         *   Gradians:
         *      Less common, but useful in specific fields or applications requiring fine granularity in
         *      angle measurements without resorting to decimal places.
         *
         *   The conversion functions enable physics simulation developers to work in their preferred unit of
         *      measurement and convert between units as needed for specific calculations, animations, or to
         *      interface with libraries and APIs that require a specific unit of angle measurement.
        */
        public const float ZeroTolerance = 1e-6f;

        public const float DEG_TO_RAD = (float)(Math.PI / 180.0);
        public const float RAD_TO_DEG = (float)(180.0 / Math.PI);
        public const float RADS_IN_CIRCLE = (float)(2 * Math.PI);

        /// <summary>
        /// 45 (looking right) degrees.... but in radians.
        /// </summary>
        public const float RADIANS_45 = 45 * DEG_TO_RAD;

        /// <summary>
        /// 90 (looking right) degrees.... but in radians.
        /// </summary>
        public const float RADIANS_90 = 90 * DEG_TO_RAD;

        /// <summary>
        /// 270 degrees (looking left) .... but in radians.
        /// </summary>
        public const float RADIANS_270 = 270 * DEG_TO_RAD;

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="rad">Given radians to convert to degrees.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadToDeg(float radians) => radians * RAD_TO_DEG;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="deg">Given degrees to convert to radians.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegToRad(float degrees) => degrees * DEG_TO_RAD;

        /// <summary>
        /// Converts cardinal x,y to degrees.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CardinalToDeg(float x, float y) => RadToDeg((float)Math.Atan2(y, x));

        /// <summary>
        /// Converts cardinal x,y to radians.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CardinalToRad(float x, float y) => (float)Math.Atan2(y, x);


        /// <summary>
        /// Converts radians to cardinal x,y.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static (float X, float Y) RadToCardinal(float radians)
            => ((float)Math.Cos(radians), (float)Math.Sin(radians));

        /// <summary>
        /// Restrict a value to be within a specified range.Useful for keeping objects within boundaries.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Interpolate between two points or values.Useful for animations, smoothing movements, or gradual transitions.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static float Lerp(float from, float to, float amount)
        {
            return (1 - amount) * from + amount * to;
        }

        /// <summary>
        /// Returns whether the value is near to one.
        /// </summary>
        public static bool IsOne(float a) => IsNearZero(a - 1.0f);

        /// <summary>
        /// Returns whether the value is near to zero.
        /// </summary>
        public static bool IsNearZero(float a)
            => Math.Abs(a) < ZeroTolerance;

        /// <summary>
        /// A value specifying the approximation of π which is 180 degrees.
        /// </summary>
        public const float Pi = (float)Math.PI;

        /// <summary>
        /// A value specifying the approximation of 2π which is 360 degrees.
        /// </summary>
        public const float TwoPi = (float)(2 * Math.PI);

        /// <summary>
        /// A value specifying the approximation of π/2 which is 90 degrees.
        /// </summary>
        public const float PiOverTwo = (float)(Math.PI / 2);

        /// <summary>
        /// A value specifying the approximation of π/4 which is 45 degrees.
        /// </summary>
        public const float PiOverFour = (float)(Math.PI / 4);

        /// <summary>
        /// Checks if a - b are almost equals within a float epsilon.
        /// </summary>
        /// <param name="a">The left value to compare.</param>
        /// <param name="b">The right value to compare.</param>
        /// <param name="epsilon">Epsilon value</param>
        /// <returns><c>true</c> if a almost equal to b within a float epsilon, <c>false</c> otherwise</returns>
        public static bool WithinEpsilon(float a, float b, float epsilon)
        {
            float num = a - b;
            return -epsilon <= num && num <= epsilon;
        }

        /// <summary>
        /// Converts revolutions to degrees.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RevolutionsToDegrees(float revolution)
        {
            return revolution * 360.0f;
        }

        /// <summary>
        /// Converts revolutions to radians.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RevolutionsToRadians(float revolution)
        {
            return revolution * TwoPi;
        }

        /// <summary>
        /// Converts revolutions to gradians.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RevolutionsToGradians(float revolution)
        {
            return revolution * 400.0f;
        }

        /// <summary>
        /// Converts degrees to revolutions.
        /// </summary>
        /// <param name="degree">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float DegreesToRevolutions(float degree)
        {
            return degree / 360.0f;
        }

        /// <summary>
        /// Converts radians to revolutions.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RadiansToRevolutions(float radian)
        {
            return radian / TwoPi;
        }

        /// <summary>
        /// Converts radians to gradians.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RadiansToGradians(float radian)
        {
            return radian * (200.0f / Pi);
        }

        /// <summary>
        /// Converts gradians to revolutions.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float GradiansToRevolutions(float gradian)
        {
            return gradian / 400.0f;
        }

        /// <summary>
        /// Converts gradians to degrees.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float GradiansToDegrees(float gradian)
        {
            return gradian * (9.0f / 10.0f);
        }

        /// <summary>
        /// Converts gradians to radians.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float GradiansToRadians(float gradian)
        {
            return gradian * (Pi / 200.0f);
        }

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The result of clamping a value between min and max</returns>
        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Smoothstep
        /// </remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        public static float SmoothStep(float amount)
        {
            return amount <= 0 ? 0
                : amount >= 1 ? 1
                : amount * amount * (3 - 2 * amount);
        }

        /// <summary>
        /// Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Smoothstep
        /// </remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        public static float SmootherStep(float amount)
        {
            return amount <= 0 ? 0
                : amount >= 1 ? 1
                : amount * amount * amount * (amount * (amount * 6 - 15) + 10);
        }

        /// <summary>
        /// Calculates the modulo of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulo">The modulo.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static float Mod(float value, float modulo)
        {
            if (modulo == 0.0f)
            {
                return value;
            }

            return value % modulo;
        }

        /// <summary>
        /// Calculates the modulo 2*PI of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static float Mod2PI(float value)
        {
            return Mod(value, TwoPi);
        }

        /// <summary>
        /// Wraps the specified value into a range [min, max]
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>Result of the wrapping.</returns>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        public static int Wrap(int value, int min, int max)
        {
            if (min > max)
                throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");

            // Code from http://stackoverflow.com/a/707426/1356325
            int range_size = max - min + 1;

            if (value < min)
                value += range_size * ((min - value) / range_size + 1);

            return min + (value - min) % range_size;
        }

        /// <summary>
        /// Gauss function.
        /// http://en.wikipedia.org/wiki/Gaussian_function#Two-dimensional_Gaussian_function
        /// </summary>
        /// <param name="amplitude">Curve amplitude.</param>
        /// <param name="x">Position X.</param>
        /// <param name="y">Position Y</param>
        /// <param name="centerX">Center X.</param>
        /// <param name="centerY">Center Y.</param>
        /// <param name="sigmaX">Curve sigma X.</param>
        /// <param name="sigmaY">Curve sigma Y.</param>
        /// <returns>The result of Gaussian function.</returns>
        public static float Gauss(float amplitude, float x, float y, float centerX, float centerY, float sigmaX, float sigmaY)
        {
            var cx = x - centerX;
            var cy = y - centerY;

            var componentX = cx * cx / (2 * sigmaX * sigmaX);
            var componentY = cy * cy / (2 * sigmaY * sigmaY);

            return (float)(amplitude * Math.Exp(-(componentX + componentY)));
        }
    }
}
