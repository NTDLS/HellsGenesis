using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Si.Library.Types.Geometry
{
    internal class SiMath
    {
        /// <summary>
        /// Restrict a value to be within a specified range.Useful for keeping objects within boundaries.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        /// Interpolate between two points or values.Useful for animations, smoothing movements, or gradual transitions.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static double Lerp(double start, double end, double amount)
        {
            return start + (end - start) * amount;
        }
    }
}
