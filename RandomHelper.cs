using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    public static class RandomHelper
    {
        private static Random randomSeed = new Random();

        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder RandStr = new StringBuilder(size);

            int Start = (lowerCase) ? 97 : 65;

            for (int i = 0; i < size; i++)
                RandStr.Append((char)(26 * randomSeed.NextDouble() + Start));

            return RandStr.ToString();
        }

        public static int RandomInt(int min, int max)
        {
            return randomSeed.Next(min, max + 1);
        }

        public static double RandomDouble()
        {
            return randomSeed.NextDouble();
        }

        public static double RandomNumber(int min, int max, int digits)
        {
            return Math.Round(randomSeed.Next(min, max - 1) + randomSeed.NextDouble(), digits);
        }

        public static bool RandomBool()
        {
            return (randomSeed.NextDouble() > 0.5);
        }

        public static DateTime RandomDate()
        {
            return RandomDate(new DateTime(1900, 1, 1), DateTime.Now);
        }

        public static DateTime RandomDate(DateTime from, DateTime to)
        {
            TimeSpan range = new TimeSpan(to.Ticks - from.Ticks);
            return from + new TimeSpan((long)(range.Ticks * randomSeed.NextDouble()));
        }
    }
}
