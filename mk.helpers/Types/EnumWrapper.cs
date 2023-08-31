using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers.Types
{
    public class EnumWrapper<T> where T : struct, IConvertible
    {
        public int Key { get; set; }
        public string Name { get; set; }

        public T GetEnum()
        {
            return (T)(object)Key;
        }

        public EnumWrapper()
        {

        }
        public EnumWrapper(T enumValue)
        {

            if (!typeof(T).IsEnum)
                throw new ArgumentException("Not an enum");

            Key = Convert.ToInt32(enumValue);
            Name = enumValue.ToString();
        }

        public static T GetEnum(EnumWrapper<T> enumWrapper)
        {
            if (enumWrapper == null)
                return default;
            return (T)(object)enumWrapper.Key;
        }


        public static EnumWrapper<T> FromValue(T enumValue)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Not an enum");
            return new EnumWrapper<T>
            {
                Key = Convert.ToInt32(enumValue),
                Name = enumValue.ToString(),
            };
        }


        public static EnumWrapper<T> FromName(string enumValue)
        {
            if (EnumHelper.TryParse(enumValue, out T result))
                return FromValue(result);

            if (EnumHelper.TryParse(enumValue.ToUpper(), out T result2))
                return FromValue(result2);
            if (EnumHelper.TryParse(enumValue.ToLower(), out T result3))
                return FromValue(result3);

            return null;
        }

    }
}
