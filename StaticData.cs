using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace mk.helpers
{
    /// <summary>
    /// A static class for managing and retrieving static data stored in a dictionary.
    /// </summary>
    public static class StaticData
    {
        private static Dictionary<string, string> _data = new Dictionary<string, string>();

        /// <summary>
        /// Adds or updates a key-value pair in the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data.</param>
        /// <param name="value">The value of the data.</param>
        public static void Add(string key, string value)
        {
            if (_data.ContainsKey(key))
                _data.Remove(key);

            _data.Add(key, value);
        }

        /// <summary>
        /// Adds or updates an object of type T in the static data dictionary.
        /// </summary>
        /// <typeparam name="T">The type of object to be added.</typeparam>
        /// <param name="key">The key of the data.</param>
        /// <param name="value">The object value to be added.</param>
        public static void AddObject<T>(string key, T value) where T : class
        {
            Add(key, JsonConvert.SerializeObject(value, Formatting.None));
        }

        /// <summary>
        /// Adds an array of key-value pairs to the static data dictionary.
        /// </summary>
        /// <param name="keyValuePairs">An array of key-value pairs to be added.</param>
        public static void Add(KeyValuePair<string, string>[] keyValuePairs)
        {
            foreach (var pair in keyValuePairs)
                Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Adds a dictionary of key-value pairs to the static data dictionary.
        /// </summary>
        /// <param name="data">A dictionary of key-value pairs to be added.</param>
        public static void Add(Dictionary<string, string> data)
        {
            foreach (var pair in data)
                Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// Retrieves the value associated with the specified key from the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The value associated with the specified key, or null if the key does not exist.</returns>
        public static string Get(string key)
        => _data.ContainsKey(key) ? _data[key] : null;

        /// <summary>
        /// Retrieves the integer value associated with the specified key from the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The integer value associated with the specified key, or null if the key does not exist or is not a valid integer.</returns>
        public static int? GetInt(string key)
        {
            var d = 0;
            var item = Get(key);
            if (!int.TryParse(item, out d))
                return null;
            return d;
        }

        /// <summary>
        /// Retrieves the Int16 value associated with the specified key from the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The Int16 value associated with the specified key, or null if the key does not exist or is not a valid Int16.</returns>
        public static Int16? GetInt16(string key)
        {
            Int16 d = 0;
            var item = Get(key);
            if (!Int16.TryParse(item, out d))
                return null;
            return d;
        }

        /// <summary>
        /// Retrieves the Int32 value associated with the specified key from the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The Int32 value associated with the specified key, or null if the key does not exist or is not a valid Int32.</returns>
        public static Int32? GetInt32(string key)
        {
            Int32 d = 0;
            var item = Get(key);
            if (!Int32.TryParse(item, out d))
                return null;
            return d;
        }

        /// <summary>
        /// Retrieves the Int64 value associated with the specified key from the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The Int64 value associated with the specified key, or null if the key does not exist or is not a valid Int64.</returns>
        public static Int64? GetInt64(string key)
        {
            Int64 d = 0;
            var item = Get(key);
            if (!Int64.TryParse(item, out d))
                return null;
            return d;
        }

        /// <summary>
        /// Retrieves the Decimal value associated with the specified key from the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The Decimal value associated with the specified key, or null if the key does not exist or is not a valid Decimal.</returns>
        public static Decimal? GetDecimal(string key)
        {
            Decimal d = 0;
            var item = Get(key);
            if (!Decimal.TryParse(item, out d))
                return null;
            return d;
        }

        /// <summary>
        /// Retrieves the Boolean value associated with the specified key from the static data dictionary.
        /// </summary>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The Boolean value associated with the specified key, or false if the key does not exist or is not a valid Boolean.</returns>
        public static bool GetBoolean(string key)
        {
            var item = Get(key)?.ToLower();
            return item == "true" || item == "1";
        }

        /// <summary>
        /// Retrieves the object of type T associated with the specified key from the static data dictionary.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The key of the data to retrieve.</param>
        /// <returns>The object of type T associated with the specified key, or null if the key does not exist or if deserialization fails.</returns>
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
