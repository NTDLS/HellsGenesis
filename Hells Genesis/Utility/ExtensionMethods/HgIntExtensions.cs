namespace HG.Utility.ExtensionMethods
{
    internal static class HgIntExtensions
    {
        /// <summary>
        /// Clips a value to a min/max value.
        /// </summary>
        public static int Box(this int value, int minValue, int maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }
    }
}
