using System;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for working with integer values.
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Converts zero to null for non-nullable integer values.
        /// </summary>
        /// <param name="value">The integer value to convert.</param>
        /// <returns>Null if the value is zero; otherwise, the original integer value.</returns>
        public static int? ZeroToNull(this int value)
        {
            return value == 0 ? null : (int?)value;
        }

        /// <summary>
        /// Converts zero to null for nullable integer values.
        /// </summary>
        /// <param name="value">The nullable integer value to convert.</param>
        /// <returns>Null if the value is null or zero; otherwise, the original nullable integer value.</returns>
        public static int? ZeroToNull(this int? value)
        {
            return value == 0 ? null : value;
        }
    }
}
