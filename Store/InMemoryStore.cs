using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.Store
{
    public static class InMemoryStore
    {
        private static ConcurrentDictionary<string, StoreValue> _storage { get; set; } = new ConcurrentDictionary<string, StoreValue>();
        private static object _lock = new object();

        public static void Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            lock (_lock)
            {
                var data = new StoreValue
                {
                    Value = value,
                };
                if (expiry != null)
                    data.ExpiresOn = DateTime.UtcNow.Add(expiry.Value);

                _storage[key] = data;
            }
        }

        public static T Get<T>(string key, T setOnNull)
        {
            return Get<T>(key, () =>
            {
                return setOnNull;
            });
        }

        public static T Get<T>(string key, Func<T>? setOnNull = null)
        {
            var find = _storage.ContainsKey(key) ? _storage[key] : null;

            if (find != null && find.ExpiresOn != null && find.ExpiresOn < DateTime.UtcNow)
                find = null;


            if (find == null && setOnNull != null)
            {
                var res = setOnNull.Invoke();
                if (res != null)
                {
                    Set<T>(key, res);
                    return res;
                }
                return default(T)!;
            }
            if (find == null || find.Value.GetType() != typeof(T))
                return default(T)!;

            return (T)find.Value;
        }


        public static TimeSpan? GetExpiry(string key)
        {
            var find = _storage.ContainsKey(key) ? _storage[key] : null;
            if (find == null || find.ExpiresOn == null)
                return null;

            return find.ExpiresOn - DateTime.UtcNow;
        }

        public static bool Invalidate(string key)
        {
            lock (_lock)
            {
                if (_storage.ContainsKey(key))
                {
                    _storage.TryRemove(key, out var temp);
                    return true;
                }
                Task.Run(() => CleanUp());
                return false;
            }

        }

        public static void InvalidateAll()
        {
            lock (_lock)
                _storage.Clear();
        }

        private static void CleanUp()
        {
            lock (_lock)
            {
                var dt = DateTime.UtcNow;
                var expiredKeys = _storage.Where(d => d.Value != null && d.Value?.ExpiresOn < dt).Select(d => d.Key).ToList();
                Parallel.ForEach(expiredKeys, (key) =>
                {
                    if (_storage.ContainsKey(key))
                        _storage.TryRemove(key, out var temp);
                });
            }
        }

        public static int TotalEntries()
        {
            CleanUp();
            return _storage.Count;
        }

        //public static long CalculateEstimatedMemoryUsageBytes()
        //{
        //    try
        //    {
        //        BinaryFormatter bf = new BinaryFormatter();
        //        MemoryStream ms = new MemoryStream();
        //        byte[] Array;
        //        bf.Serialize(ms, _storage);
        //        Array = ms.ToArray();
        //        return Array.Length;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return 0;
        //}

    }

    [Serializable]
    public class StoreValue
    {
        public object Value { get; set; }
        public DateTime? ExpiresOn { get; set; }
    }
}
