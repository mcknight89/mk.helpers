using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    public static class StaticData
    {
        private static Dictionary<string, string> _data = new Dictionary<string, string>();
        public static void Add(string key, string value)
        {
            if (_data.ContainsKey(key))
                _data.Remove(key);

            _data.Add(key, value);
        }

        public static void AddObject<T>(string key, T value) where T : class
        {
            Add(key, JsonConvert.SerializeObject(value, Formatting.None));
        }

        public static void Add(KeyValuePair<string, string>[] keyValuePairs)
        {
            foreach (var pair in keyValuePairs)
                Add(pair.Key, pair.Value);
        }
        public static void Add(Dictionary<string, string> data)
        {
            foreach (var pair in data)
                Add(pair.Key, pair.Value);
        }
        public static string Get(string key)
        => _data.ContainsKey(key) ? _data[key] : null;




        public static int? GetInt(string key)
        {
            var d = 0;
            var item = Get(key);
            if (!int.TryParse(item, out d))
                return null;
            return d;
        }

        public static Int16? GetInt16(string key)
        {
            Int16 d = 0;
            var item = Get(key);
            if (!Int16.TryParse(item, out d))
                return null;
            return d;
        }

        public static Int32? GetInt32(string key)
        {
            Int32 d = 0;
            var item = Get(key);
            if (!Int32.TryParse(item, out d))
                return null;
            return d;
        }

        public static Int64? GetInt64(string key)
        {
            Int64 d = 0;
            var item = Get(key);
            if (!Int64.TryParse(item, out d))
                return null;
            return d;
        }


        public static Decimal? GetDecimal(string key)
        {
            Decimal d = 0;
            var item = Get(key);
            if (!Decimal.TryParse(item, out d))
                return null;
            return d;
        }


        public static bool GetBoolean(string key)
        {
            var item = Get(key)?.ToLower();
            return item == "true" || item == "1";
        }



        public static T GetObject<T>(string key) where T : class
        {
            var value = _data[key];
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                // no logging yet
            }

            return default(T);
        }
    }


}
