using System;
using System.Globalization;
using System.Linq;

namespace mk.helpers
{
    /// <summary>
    /// Reads and writes numbers as text, always with the invariant culture.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Reads a float, or returns the fallback when the text is null, empty or not a number.
        /// </summary>
        public static float ToFloat(this string text, float fallback = 0)
        {
            return text.TryToFloat(out var value) ? value : fallback;
        }

        /// <summary>
        /// Tries to read a float.
        /// </summary>
        public static bool TryToFloat(this string text, out float value)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Reads a double, or returns the fallback when the text is null, empty or not a number.
        /// </summary>
        public static double ToDouble(this string text, double fallback = 0)
        {
            return text.TryToDouble(out var value) ? value : fallback;
        }

        /// <summary>
        /// Tries to read a double.
        /// </summary>
        public static bool TryToDouble(this string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Reads an int, or returns the fallback when the text is null, empty or not a whole number.
        /// </summary>
        public static int ToInt(this string text, int fallback = 0)
        {
            return text.TryToInt(out var value) ? value : fallback;
        }

        /// <summary>
        /// Tries to read an int.
        /// </summary>
        public static bool TryToInt(this string text, out int value)
        {
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Reads a list of floats, e.g. "22 22 13". Throws when a part isn't a number.
        /// </summary>
        public static float[] ToFloats(this string text, char separator = ' ')
        {
            if (string.IsNullOrWhiteSpace(text))
                return new float[0];

            return text.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture)).ToArray();
        }

        /// <summary>
        /// Reads a list of ints, e.g. "1 2 3". Throws when a part isn't a whole number.
        /// </summary>
        public static int[] ToInts(this string text, char separator = ' ')
        {
            if (string.IsNullOrWhiteSpace(text))
                return new int[0];

            return text.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x, NumberStyles.Integer, CultureInfo.InvariantCulture)).ToArray();
        }

        /// <summary>
        /// Writes a float with the invariant culture.
        /// </summary>
        public static string ToInvariant(this float value, string format = null)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Writes a double with the invariant culture.
        /// </summary>
        public static string ToInvariant(this double value, string format = null)
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
