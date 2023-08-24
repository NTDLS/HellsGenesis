using System;

namespace HG.Engine
{
    internal class HgRandom
    {
        public static Random Random = new();

        public static bool ChanceIn(int n)
        {
            return (Random.Next(0, n * 10) % n) == n / 2;
        }

        public static bool FlipCoin()
        {
            return Random.Next(0, 1000) >= 500;
        }

        public static double RandomNumber(double min, double max)
        {
            return Random.NextDouble() * (max - min) + min;
        }

        public static int RandomNumber(int min, int max)
        {
            return Random.Next(min, 1000) % max;
        }

        /// <summary>
        /// This POS is just awful. It doesnt even accept negative input. Or respect it
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomNumberNegative(int min, int max)
        {
            if (FlipCoin())
            {
                return -(Random.Next(min, 1000) % max);
            }
            return Random.Next(min, 1000) % max;
        }
    }
}
