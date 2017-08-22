using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soujiwolf.Utility
{
    public static class RNG
    {
        public static Random Random { get; set; }

        static RNG()
        {
            Reseed();
        }

        public static void Reseed()
        {
            Random = new Random();

        }

        /// <summary>
        /// Returns the value of Random.Next();
        /// </summary>
        /// <returns></returns>
        public static int Next()
        {
            return Random.Next();
        }

        /// <summary>
        /// Returns the value of Random.Next(maxValue);
        /// </summary>
        /// <param name="maxValue">Exclusive maximum value</param>
        /// <returns></returns>
        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static double NextDouble()
        {
            return Random.NextDouble();
        }

        /// <summary>
        /// Returns the value of Random.Next(minValue, maxValue)
        /// </summary>
        /// <param name="minValue">Inclusive minimum</param>
        /// <param name="maxValue">Exclusive maximum</param>
        public static int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Given a chance (0 - 1), return a bool
        /// </summary>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static bool Roll(double chance)
        {
            var value = Next(0, 100) / 100d;
            return (1d - chance) <= value;
        }

        public static T Next<T>(List<T> choices, List<double> weights)
        {
            if (choices.Count != weights.Count)
                throw new InvalidOperationException("Choices and their weights must be the same length.");

            var sum = 0d;
            foreach (double d in weights)
                sum += d;
            if (1 - sum > 0)
                throw new InvalidOperationException("Sum of all weights is not 1!");
            // Generate a random double
            double value = NextDouble();

            double weight_sum = 0d;
            for (var i = 0; i < weights.Count; i++)
            {
                weight_sum += weights[i];
                if (value <= weight_sum)
                    return choices[i];
            }
            return default(T);
        }

        public static T Next<T>(List<Tuple<double,T>> choices)
        {
            var sum = 0d;
            foreach (var item in choices)
                sum += item.Item1;
            if (1 - sum > 0)
                throw new InvalidOperationException("Sum of all weights is not 1!");

            // Generate a random double
            double value = NextDouble();

            double weight_sum = 0d;
            for (var i = 0; i < choices.Count; i++)
            {
                weight_sum += choices[i].Item1;
                if (value <= weight_sum)
                    return choices[i].Item2;
            }
            return default(T);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}