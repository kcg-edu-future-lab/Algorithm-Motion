using System;

namespace AlgoMotionLib
{
    public static class NumberHelper
    {
        internal static Random Random { get; } = new Random();

        public static double NextDouble(double minValue, double maxValue)
        {
            return minValue + (maxValue - minValue) * Random.NextDouble();
        }

        public static int Mod(this int i, int n)
        {
            var j = i % n;
            return j >= 0 ? j : j + n;
        }
    }
}
