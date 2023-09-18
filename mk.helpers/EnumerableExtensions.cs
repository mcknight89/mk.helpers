
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.Types
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IEnumerable{T}"/> collections.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Implements paging on an <see cref="IEnumerable{T}"/> source.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A <see cref="PagedResult{T}"/> containing the paged data.</returns>
        public static PagedResult<T> Paged<T>(this IEnumerable<T> source, int page, int pageSize) where T : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var total = source.Count();
            var filtered = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>()
            {
                Results = filtered,
                CurrentPage = page,
                RowCount = total,
                PageCount = (total / pageSize) +1,
                PageSize = filtered.Count()
            };
        }

        /// <summary>
        /// Returns distinct elements from a sequence by a specified key selector.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="keySelector">A function to extract the key for each element.</param>
        /// <returns>An <see cref="IEnumerable{TSource}"/> containing distinct elements.</returns>
        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    var knownKeys = new HashSet<TKey>();
        //    return source.Where(element => knownKeys.Add(keySelector(element)));
        //}

        ///// <summary>
        ///// Appends an item to an <see cref="IEnumerable{T}"/> sequence.
        ///// </summary>
        ///// <typeparam name="T">The type of elements in the collection.</typeparam>
        ///// <param name="sequence">The source sequence.</param>
        ///// <param name="item">The item to append.</param>
        ///// <returns>An <see cref="IEnumerable{T}"/> with the appended item.</returns>
        //public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, T item)
        //    => sequence?.Concat(item.Yield());

        ///// <summary>
        ///// Prepends an item to an <see cref="IEnumerable{T}"/> sequence.
        ///// </summary>
        ///// <typeparam name="T">The type of elements in the collection.</typeparam>
        ///// <param name="sequence">The source sequence.</param>
        ///// <param name="item">The item to prepend.</param>
        ///// <returns>An <see cref="IEnumerable{T}"/> with the prepended item.</returns>
        //public static IEnumerable<T> Prepend<T>(this IEnumerable<T> sequence, T item)
        //    => item?.Yield().Concat(sequence);

        /// <summary>
        /// Yields a single item as an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="obj">The item to yield.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the item.</returns>
        public static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }

        /// <summary>
        /// Checks if an item is any of a given list of items.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="source">The source item to check.</param>
        /// <param name="list">The list of items to check against.</param>
        /// <returns><c>true</c> if the source item is any of the items in the list; otherwise, <c>false</c>.</returns>
        public static bool IsAnyOf<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            return list.Contains(source);
        }


        /// <summary>
        /// Sorts a collection based on the order of a reference collection using specified key selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source and reference collections.</typeparam>
        /// <typeparam name="TKey">The type of the sorting key.</typeparam>
        /// <param name="source">The source collection to be sorted.</param>
        /// <param name="orderReference">The reference collection that defines the desired order.</param>
        /// <param name="sourceKeySelector">A function to extract the sorting key from elements in the source collection.</param>
        /// <param name="compareKeySelector">A function to extract the sorting key from elements in the reference collection for comparison.</param>
        /// <returns>A new IEnumerable<TSource> sorted based on the order in the reference collection.</returns>
        /// <remarks>
        /// This method sorts the source collection based on the order of elements in the reference collection.
        /// It allows you to specify different key selectors for the source and reference collections.
        /// </remarks>
        public static IEnumerable<TSource> SortByReference<TSource, TKey>(
            this IEnumerable<TSource> source,
            IEnumerable<TSource> orderReference,
            Func<TSource, TKey> sourceKeySelector,
            Func<TSource, TKey> compareKeySelector)
        {
            var keyToIndex = orderReference
                .Select((item, index) => new { Key = compareKeySelector(item), Index = index })
                .ToDictionary(x => x.Key, x => x.Index);

            return source.OrderBy(item =>
            {
                if (keyToIndex.TryGetValue(sourceKeySelector(item), out var index))
                {
                    return index;
                }
                // Handle missing keys by placing them at the end (you can use orderReference.Count or any other suitable value)
                return orderReference.Count();
            });
        }

        /// <summary>
        /// Sorts a collection of strings based on the order of a reference collection.
        /// </summary>
        /// <param name="listToSort">The list of strings to be sorted.</param>
        /// <param name="orderReference">The reference list of strings that defines the desired order.</param>
        /// <returns>A new List<string> sorted based on the order in the reference collection.</returns>
        public static List<string> SortByReference(this List<string> listToSort, List<string> orderReference)
        {
            return listToSort
                .OrderBy(item =>
                {
                    int index = orderReference.IndexOf(item);
                    return index != -1 ? index : orderReference.Count;
                })
                .ToList();
        }



        /// <summary>
        /// Filters items from a source collection based on keys found in a reference collection using specified key selectors.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source and reference collections.</typeparam>
        /// <typeparam name="TKey">The type of the key used for filtering.</typeparam>
        /// <param name="source">The source collection from which to filter items.</param>
        /// <param name="orderReference">The reference collection containing the keys for filtering.</param>
        /// <param name="sourceKeySelector">A function to extract the key from elements in the source collection.</param>
        /// <param name="compareKeySelector">A function to extract the key from elements in the reference collection for comparison.</param>
        /// <returns>An IEnumerable&lt;TSource&gt; containing items from the source collection that have keys present in the reference collection.</returns>
        /// <remarks>
        /// This method filters items from the source collection based on keys found in the reference collection.
        /// It allows you to specify different key selectors for the source and reference collections.
        /// </remarks>
        public static IEnumerable<TSource> FilterByReference<TSource, TKey>(
            this IEnumerable<TSource> source,
            IEnumerable<TSource> orderReference,
            Func<TSource, TKey> sourceKeySelector,
            Func<TSource, TKey> compareKeySelector)
        {
            var referenceKeys = new HashSet<TKey>(orderReference.Select(compareKeySelector));

            return source.Where(item => referenceKeys.Contains(sourceKeySelector(item)));
        }

    }
}