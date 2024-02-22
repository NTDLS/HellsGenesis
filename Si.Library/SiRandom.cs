namespace Si.Library
{
    public class SiRandom
    {
        public static Random Generator = new();

        /// <summary>
        /// Returns the given value with a given variance added or subtracted (at random).
        /// </summary>
        /// <param name="value"></param>
        /// <param name="variancePercentWholeNumber"></param>
        /// <returns></returns>
        public static double Variance(double value, double variancePercentDecimal)
            => value + RandomSign(value * variancePercentDecimal);

        /// <summary>
        /// 50/50 chance to return a positive/negative of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double RandomSign(double value)
            => FlipCoin() ? value : -value;

        public static double OneOf(double one, double two)
            => OneOf([one, two]);

        public static double OneOf(double one, double two, double three)
            => OneOf([one, two, three]);

        public static double OneOf(double one, double two, double three, double four)
            => OneOf([one, two, three, four]);

        public static double OneOf(double[] values)
            => values[Between(0, values.Length - 1)];

        public static bool ChanceIn(int chanceIn, int outOf)
            => Generator.Next(1, outOf + 1) <= chanceIn;

        public static bool PercentChance(int percentageWholeNumber)
            => (Generator.NextDouble() * 100) <= percentageWholeNumber;

        public static bool FlipCoin() => (Generator.Next(2) == 0);

        public static double Between(double minValue, double maxValue)
            => minValue + (maxValue - minValue) * Generator.NextDouble();

        public static int Between(int minValue, int maxValue)
            => Generator.Next(minValue, maxValue + 1);
    }
}