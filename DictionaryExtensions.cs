using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers
{
   
    public static class DictionaryExtensions
    {
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            if (source == null) throw new Exception("Source is null");
            if (keySelector == null) throw new Exception("Key is null");
            if (elementSelector == null) throw new Exception("Selector is null");

            ConcurrentDictionary<TKey, TElement> d = new ConcurrentDictionary<TKey, TElement>();
            foreach (TSource element in source) d.TryAdd(keySelector(element), elementSelector(element));
            return d;
        }

        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new Exception("Source is null");
            if (keySelector == null) throw new Exception("Key is null");
            if (elementSelector == null) throw new Exception("Selector is null");

            ConcurrentDictionary<TKey, TElement> d = new ConcurrentDictionary<TKey, TElement>(comparer);
            foreach (TSource element in source) d.TryAdd(keySelector(element), elementSelector(element));
            return d;
        }
        public static ConcurrentBag<TSource> ToConcurrentBag<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new Exception("Source is null");
            return new ConcurrentBag<TSource>(source);
        }

        public static void IncrementAt<T>(this Dictionary<T, int> dictionary, T index)
        {
            int count = 0;
            dictionary.TryGetValue(index, out count);
            dictionary[index] = ++count;
        }

        public static void IncrementAt<T>(this ConcurrentDictionary<T, int> dictionary, T index)
        {
            int count = 0;
            dictionary.TryGetValue(index, out count);
            dictionary[index] = ++count;
        }

        public static TValue TryGet<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary?.ContainsKey(key) == true)
                return dictionary[key];
            return default(TValue);
        }

    }
}
