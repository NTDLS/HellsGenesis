using System.Runtime.CompilerServices;

namespace Si.Library.Mathematics.Geometry
{
    public static class SiVectorExtensions
    {
        /// <summary>
        /// Calculate the angle between two points in unsigned degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this SiVector from, SiVector to)
        {
            var radians = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (SiMath.RadToDeg(radians) + 360.0f) % 360.0f;
        }

        /// <summary>
        /// Calculate the angle between two points in signed degrees.
        /// </summary>
        /// <param name="from">The object from which the calcualtion is based.</param>
        /// <param name="to">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(this SiVector from, SiVector to)
        {
            var angle = from.AngleToInUnsignedDegrees(to);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Calculate the angle between two points in signed radians.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedRadians(this SiVector from, SiVector to)
            => (float)Math.Atan2(to.Y - from.Y, to.X - from.X);

        /// <summary>
        /// Calculate the angle between two points in unsigned radians.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedRadians(this SiVector from, SiVector to)
        {
            var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            if (angle < 0)
            {
                angle += 2 * (float)Math.PI; // Convert negative angles to positive by adding 2π
            }
            return angle;
        }
    }
}
