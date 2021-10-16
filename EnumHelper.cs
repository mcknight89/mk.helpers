using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    public static class EnumHelper
    {
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
