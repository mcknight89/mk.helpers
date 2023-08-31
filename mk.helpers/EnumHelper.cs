using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    /// <summary>
    /// Provides utility methods for working with enumerations.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Parses a string representation of an enumeration value to its corresponding enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="text">The string representation of the enumeration value to parse.</param>
        /// <returns>The parsed enumeration value.</returns>
        /// <remarks>
        /// This method attempts to parse the input text into the specified enumeration type. It supports variations in formatting
        /// such as underscores and spaces, and attempts to match the case-insensitive enum member names.
        /// </remarks>
        public static T Parse<T>(string text) where T : struct, IConvertible
        {
            T result = default(T);
            text = text?.Trim();
            if (Enum.TryParse(text, out result))
                return result;
            if (Enum.TryParse(text?.Replace("_", ""), out result))
                return result;
            if (Enum.TryParse(text?.Replace("_", " ")?.ToTitleCase()?.Replace(" ", ""), out result))
                return result;
            return result;
        }

        /// <summary>
        /// Tries to parse a string representation of an enumeration value to its corresponding enumeration value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="text">The string representation of the enumeration value to parse.</param>
        /// <param name="result">When this method returns, contains the parsed enumeration value if the parsing was successful.</param>
        /// <returns><c>true</c> if the parsing was successful; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method attempts to parse the input text into the specified enumeration type. It supports variations in formatting
        /// such as underscores and spaces, and attempts to match the case-insensitive enum member names.
        /// </remarks>
        public static bool TryParse<T>(string text, out T result) where T : struct, IConvertible
        {
            text = text?.Trim();
            if (Enum.TryParse(text, out result))
                return true;
            if (Enum.TryParse(text?.Replace("_", ""), out result))
                return true;
            if (Enum.TryParse(text?.Replace("_", " ")?.ToTitleCase()?.Replace(" ", ""), out result))
                return true;
            return false;
        }
    }
}
