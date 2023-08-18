namespace AI2D.Types.ExtensionMethods
{
    public static class DoubleExtensions
    {
        public static double Box(this double value, double minValue, double maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }
    }
}