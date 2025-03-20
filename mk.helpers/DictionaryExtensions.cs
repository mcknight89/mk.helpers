using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for working with <see cref="Dictionary{TKey, TValue}"/> and <see cref="ConcurrentDictionary{TKey, TValue}"/> collections.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Converts an <see cref="IEnumerable{TSource}"/> to a <see cref="ConcurrentDictionary{TKey, TElement}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
        /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
        /// <typeparam name="TElement">The type of the dictionary values.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="keySelector">A function to extract keys from elements.</param>
        /// <param name="elementSelector">A function to extract values from elements.</param>
        /// <returns>A <see cref="ConcurrentDictionary{TKey, TElement}"/> containing the elements of the source collection.</returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            if (source == null) throw new Exception("Source is null");
            if (keySelector == null) throw new Exception("Key is null");
            if (elementSelector == null) throw new Exception("Selector is null");

            ConcurrentDictionary<TKey, TElement> d = new ConcurrentDictionary<TKey, TElement>();
            foreach (TSource element in source) d.TryAdd(keySelector(element), elementSelector(element));
            return d;
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable{TSource}"/> to a <see cref="ConcurrentDictionary{TKey, TElement}"/> using a specified comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
        /// <typeparam name="TKey">The type of the dictionary keys.</typeparam>
        /// <typeparam name="TElement">The type of the dictionary values.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="keySelector">A function to extract keys from elements.</param>
        /// <param name="elementSelector">A function to extract values from elements.</param>
        /// <param name="comparer">An equality comparer to compare keys.</param>
        /// <returns>A <see cref="ConcurrentDictionary{TKey, TElement}"/> containing the elements of the source collection.</returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new Exception("Source is null");
            if (keySelector == null) throw new Exception("Key is null");
            if (elementSelector == null) throw new Exception("Selector is null");

            ConcurrentDictionary<TKey, TElement> d = new ConcurrentDictionary<TKey, TElement>(comparer);
            foreach (TSource element in source) d.TryAdd(keySelector(element), elementSelector(element));
            return d;
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable{TSource}"/> to a <see cref="ConcurrentBag{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A <see cref="ConcurrentBag{TSource}"/> containing the elements of the source collection.</returns>
        public static ConcurrentBag<TSource> ToConcurrentBag<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new Exception("Source is null");
            return new ConcurrentBag<TSource>(source);
        }



        /// <summary>
        /// Increments the value at a specified key in a <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="T">The type of keys in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="index">The key to increment.</param>
        public static void IncrementAt<T>(this IDictionary<T, int> dictionary, T index)
        {
            int count = 0;
            dictionary.TryGetValue(index, out count);
            dictionary[index] = ++count;
        }

        /// <summary>
        /// Tries to get a value from a <see cref="IDictionary{TKey, TValue}"/> based on a key.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to look up.</param>
        /// <returns>The value associated with the key, or the default value if not found.</returns>
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary?.ContainsKey(key) == true)
                return dictionary[key];
            return default(TValue);
        }

        /// <summary>
        /// Tries to get a value from a <see cref="IDictionary{TKey, TValue}"/> based on a key.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to look up.</param>
        /// <param name="defaultValue">Default value if key is not found</param>
        /// <returns>The value associated with the key, or the default value if not found.</returns>
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary?.ContainsKey(key) == true)
                return dictionary[key];
            return defaultValue;
        }
    }
}
