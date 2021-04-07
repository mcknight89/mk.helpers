using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace mk.helpers
{
    public static class Serialization
    {
        private static Encoding _encoding = null;

        private static void SetEncoding() => SetEncoding(null);
        public static void SetEncoding(Encoding encoding)
        {
            _encoding = encoding != null ? encoding : (_encoding == null ? Encoding.Default : _encoding);
        }


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


        public static string ToJson<T>(T obj)
        {
            SetEncoding();

            return JsonConvert.SerializeObject(obj);
        }

        public static byte[] ToJsonBytes<T>(T value) where T : class
        => _encoding.GetBytes(ToJson(value));
        
        public static T FromJson<T>(string obj)
        {
            SetEncoding();

            return JsonConvert.DeserializeObject<T>(obj);
        }
        
        public static T FromJsonBytes<T>(byte[] obj) where T : class
           => FromJson<T>(_encoding.GetString(obj));


        public static T FromBson<T>(string value)
        {
            SetEncoding();

            try
            {
                var data = Encoding.GetEncoding(1252).GetBytes(value);
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
            catch (Exception)
            {
                return (T)(object)value;
            }
        }
        public static T FromBsonBytes<T>(byte[] obj) where T : class
           => FromBson<T>(_encoding.GetString(obj));

        public static string ToBson<T>(T value) where T : class
        => _encoding.GetString(ToBsonBytes(value));

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
