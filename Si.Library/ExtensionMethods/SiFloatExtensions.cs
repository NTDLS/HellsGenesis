namespace Si.Library.ExtensionMethods
{
    public static class SiFloatExtensions
    {
        /// <summary>
        /// Clips a value to a min/max value.
        /// </summary>
        public static float Clamp(this float value, float minValue, float maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }

        /// <summary>
        /// Clips a value to a max value.
        /// </summary>
        public static float Clamp(this float value, float maxValue)
        {
            if (value > maxValue) return maxValue;
            else return value;
        }
    }
}
