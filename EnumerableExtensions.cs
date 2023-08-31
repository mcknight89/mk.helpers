
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
    }
}