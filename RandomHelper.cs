using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    /// <summary>
    /// Provides utility methods for generating random values.
    /// </summary>
    public static class RandomHelper
    {
        private static Random randomSeed = new Random();

        /// <summary>
        /// Generates a random string of the specified size.
        /// </summary>
        /// <param name="size">The length of the random string to generate.</param>
        /// <param name="lowerCase">Specify whether the generated string should be in lowercase.</param>
        /// <returns>A randomly generated string.</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder RandStr = new StringBuilder(size);

            int Start = (lowerCase) ? 97 : 65;

            for (int i = 0; i < size; i++)
                RandStr.Append((char)(26 * randomSeed.NextDouble() + Start));

            return RandStr.ToString();
        }

        /// <summary>
        /// Generates a random integer within the specified range.
        /// </summary>
        /// <param name="min">The minimum value of the random integer.</param>
        /// <param name="max">The maximum value of the random integer.</param>
        /// <returns>A randomly generated integer.</returns>
        public static int RandomInt(int min, int max)
        {
            return randomSeed.Next(min, max + 1);
        }

        /// <summary>
        /// Generates a random double between 0.0 and 1.0.
        /// </summary>
        /// <returns>A randomly generated double value.</returns>
        public static double RandomDouble()
        {
            return randomSeed.NextDouble();
        }

        /// <summary>
        /// Generates a random number within the specified range with the specified number of digits.
        /// </summary>
        /// <param name="min">The minimum value of the random number.</param>
        /// <param name="max">The maximum value of the random number.</param>
        /// <param name="digits">The number of decimal digits in the generated number.</param>
        /// <returns>A randomly generated number with the specified number of digits.</returns>
        public static double RandomNumber(int min, int max, int digits)
        {
            return Math.Round(randomSeed.Next(min, max - 1) + randomSeed.NextDouble(), digits);
        }

        /// <summary>
        /// Generates a random boolean value (true or false).
        /// </summary>
        /// <returns>A randomly generated boolean value.</returns>
        public static bool RandomBool()
        {
            return (randomSeed.NextDouble() > 0.5);
        }

        /// <summary>
        /// Generates a random DateTime within the default date range.
        /// </summary>
        /// <returns>A randomly generated DateTime value.</returns>
        public static DateTime RandomDate()
        {
            return RandomDate(new DateTime(1900, 1, 1), DateTime.Now);
        }

        /// <summary>
        /// Generates a random DateTime within the specified date range.
        /// </summary>
        /// <param name="from">The minimum date of the range.</param>
        /// <param name="to">The maximum date of the range.</param>
        /// <returns>A randomly generated DateTime within the specified range.</returns>
        public static DateTime RandomDate(DateTime from, DateTime to)
        {
            TimeSpan range = new TimeSpan(to.Ticks - from.Ticks);
            return from + new TimeSpan((long)(range.Ticks * randomSeed.NextDouble()));
        }
    }
}
