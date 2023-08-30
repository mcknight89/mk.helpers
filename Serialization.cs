using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using mk.helpers.Types;

namespace mk.helpers
{
    public static class Serialization
    {
        private static Encoding _encoding = Encoding.Default;

        private static void SetEncoding() => SetEncoding(null);

        /// <summary>
        /// Sets the encoding to be used for serialization and deserialization.
        /// </summary>
        /// <param name="encoding">The encoding to be set. If null, the default encoding will be used.</param>
        public static void SetEncoding(Encoding encoding)
        {
            _encoding = encoding != null ? encoding : (_encoding == null ? Encoding.Default : _encoding);
        }

        /// <summary>
        /// Serializes the provided data into a byte array using the specified serialization format.
        /// </summary>
        /// <typeparam name="T">The type of the data being serialized.</typeparam>
        /// <param name="data">The data to be serialized.</param>
        /// <param name="type">The serialization format to use.</param>
        /// <returns>The serialized data as a byte array.</returns>
        public static byte[] Serialize<T>(T data, SerializationType type) where T : class
        {
            switch (type)
            {
                case SerializationType.Json:
                    return ToJsonBytes(data);
                case SerializationType.Bson:
                    return ToBsonBytes(data);
            }
            return null;
        }

        /// <summary>
        /// Deserializes the provided byte array into an object of the specified type using the specified serialization format.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
        /// <param name="data">The byte array to be deserialized.</param>
        /// <param name="type">The serialization format to use.</param>
        /// <returns>The deserialized object of the specified type.</returns>
        public static T Deserialize<T>(byte[] data, SerializationType type) where T : class
        {
            switch (type)
            {
                case SerializationType.Json:
                    return FromJsonBytes<T>(data);
                case SerializationType.Bson:
                    return FromBsonBytes<T>(data);
            }
            return null;
        }
        /// <summary>
        /// Converts an object to its JSON representation.
        /// </summary>
        /// <typeparam name="T">The type of the object to be converted to JSON.</typeparam>
        /// <param name="obj">The object to be converted.</param>
        /// <returns>The JSON representation of the object.</returns>
        public static string ToJson<T>(T obj)
        {
            SetEncoding();

            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Converts an object to its JSON representation as a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to be converted to JSON.</typeparam>
        /// <param name="value">The object to be converted.</param>
        /// <returns>The JSON representation of the object as a byte array.</returns>
        public static byte[] ToJsonBytes<T>(T value) where T : class
            => _encoding.GetBytes(ToJson(value));

        /// <summary>
        /// Converts a JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized from JSON.</typeparam>
        /// <param name="obj">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object of the specified type.</returns>
        public static T FromJson<T>(string obj)
        {
            SetEncoding();

            return JsonConvert.DeserializeObject<T>(obj);
        }

        /// <summary>
        /// Converts a JSON byte array to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized from JSON.</typeparam>
        /// <param name="obj">The JSON byte array to be deserialized.</param>
        /// <returns>The deserialized object of the specified type.</returns>
        public static T FromJsonBytes<T>(byte[] obj) where T : class
            => FromJson<T>(_encoding.GetString(obj));
        /// <summary>
        /// Converts a BSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized from BSON.</typeparam>
        /// <param name="value">The BSON string to be deserialized.</param>
        /// <returns>The deserialized object of the specified type.</returns>
        public static T FromBson<T>(string value)
        {
            SetEncoding();

            try
            {
                var data = _encoding.GetBytes(value);
                if (typeof(T) == typeof(string))
                    return (T)(object)value;

                using (var ms = new MemoryStream(data))
                using (var reader = new BsonDataReader(ms))
                {
                    var serializer = new JsonSerializer();
                    if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                        reader.ReadRootValueAsArray = true;

                    var obj = serializer.Deserialize<T>(reader);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                return (T)(object)value;
            }
        }

        /// <summary>
        /// Converts a BSON byte array to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized from BSON.</typeparam>
        /// <param name="obj">The BSON byte array to be deserialized.</param>
        /// <returns>The deserialized object of the specified type.</returns>
        public static T FromBsonBytes<T>(byte[] obj) where T : class
            => FromBson<T>(_encoding.GetString(obj));

        /// <summary>
        /// Converts an object to its BSON representation as a string.
        /// </summary>
        /// <typeparam name="T">The type of the object to be converted to BSON.</typeparam>
        /// <param name="value">The object to be converted.</param>
        /// <returns>The BSON representation of the object as a string.</returns>
        public static string ToBson<T>(T value) where T : class
            => _encoding.GetString(ToBsonBytes(value));

        /// <summary>
        /// Converts an object to its BSON representation as a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the object to be converted to BSON.</typeparam>
        /// <param name="value">The object to be converted.</param>
        /// <returns>The BSON representation of the object as a byte array.</returns>
        public static byte[] ToBsonBytes<T>(T value) where T : class
        {
            SetEncoding();

            using (var ms = new MemoryStream())
            using (var datawriter = new BsonDataWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(datawriter, value);
                return ms.ToArray();
            }
        }
    }
}
