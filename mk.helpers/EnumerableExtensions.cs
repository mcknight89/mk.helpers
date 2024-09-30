
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
                PageCount = (total / pageSize) + 1,
                PageSize = filtered.Count()
            };
        }


        //
        // Summary:
        //     Implements paging on an System.Collections.Generic.IAsyncEnumerable`1 source asynchronously.
        //
        // Parameters:
        //   source:
        //     The source collection.
        //
        //   page:
        //     The current page number.
        //
        //   pageSize:
        //     The number of items per page.
        //
        // Type parameters:
        //   T:
        //     The type of elements in the collection.
        //
        // Returns:
        //     A mk.helpers.Types.PagedResult`1 containing the paged data asynchronously.
        public static async Task<PagedResult<T>> PagedAsync<T>(this IAsyncEnumerable<T> source, int page, int pageSize) where T : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var skippedSource = source.SkipAsync((page - 1) * pageSize);
            var pagedSource = skippedSource.TakeAsync(pageSize);
            var list = await pagedSource.ToListAsync();
            var num = await source.CountAsync();

            return new PagedResult<T>
            {
                Results = list,
                CurrentPage = page,
                RowCount = num,
                PageCount = (int)Math.Ceiling((double)num / pageSize),
                PageSize = list.Count
            };
        }

        //
        // Summary:
        //     Skips a specified number of elements in an IAsyncEnumerable`1 source asynchronously.
        //
        // Parameters:
        //   source:
        //     The source collection.
        //
        //   count:
        //     The number of elements to skip.
        //
        // Type parameters:
        //   T:
        //     The type of elements in the collection.
        //
        // Returns:
        //     An IAsyncEnumerable`1 that contains the elements that occur after the specified index.
        public static async IAsyncEnumerable<T> SkipAsync<T>(this IAsyncEnumerable<T> source, int count)
        {
            await foreach (var item in source)
            {
                if (count > 0)
                {
                    count--;
                    continue;
                }

                yield return item;
            }
        }

        //
        // Summary:
        //     Takes a specified number of elements from an IAsyncEnumerable`1 source asynchronously.
        //
        // Parameters:
        //   source:
        //     The source collection.
        //
        //   count:
        //     The number of elements to take.
        //
        // Type parameters:
        //   T:
        //     The type of elements in the collection.
        //
        // Returns:
        //     An IAsyncEnumerable`1 that contains the specified number of elements from the start of the collection.
        public static async IAsyncEnumerable<T> TakeAsync<T>(this IAsyncEnumerable<T> source, int count)
        {
            await foreach (var item in source)
            {
                if (count-- > 0)
                {
                    yield return item;
                }
                else
                {
                    break;
                }
            }
        }

        //
        // Summary:
        //     Converts an IAsyncEnumerable`1 to a List`1 asynchronously.
        //
        // Parameters:
        //   source:
        //     The source collection.
        //
        // Type parameters:
        //   T:
        //     The type of elements in the collection.
        //
        // Returns:
        //     A Task`1 representing the asynchronous operation, containing a List`1 of elements from the source collection.
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            var list = new List<T>();
            await foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }

        //
        // Summary:
        //     Counts the number of elements in an IAsyncEnumerable`1 source asynchronously.
        //
        // Parameters:
        //   source:
        //     The source collection.
        //
        // Type parameters:
        //   T:
        //     The type of elements in the collection.
        //
        // Returns:
        //     A Task`1 representing the asynchronous operation, containing the total count of elements in the source collection.
        public static async Task<int> CountAsync<T>(this IAsyncEnumerable<T> source)
        {
            int count = 0;
            await foreach (var item in source)
            {
                count++;
            }
            return count;
        }
        /// <summary>
        /// Extension method to calculate the median of a sequence of numeric values using a selector.
        /// Supported types are int, decimal, double, and long.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source collection.</typeparam>
        /// <typeparam name="TResult">The numeric type of the values returned by the selector (int, decimal, double, long).</typeparam>
        /// <param name="source">An IEnumerable containing the elements.</param>
        /// <param name="selector">A function to extract the numeric value from each element.</param>
        /// <returns>The median value of the numeric list.</returns>
        /// <exception cref="ArgumentException">Thrown when the source sequence is empty.</exception>
        public static TResult Median<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
            where TResult : struct, IComparable<TResult>, IConvertible
        {
            if (source == null || !source.Any())
                throw new ArgumentException("The source sequence is empty or null.");

            var sortedList = source.Select(selector).OrderBy(x => x).ToList();
            int count = sortedList.Count;

            if (count % 2 == 0)
            {
                // For even-sized list, return the average of the two middle elements
                TResult midLeft = sortedList[count / 2 - 1];
                TResult midRight = sortedList[count / 2];
                return Average(midLeft, midRight);
            }
            else
            {
                // For odd-sized list, return the middle element
                return sortedList[count / 2];
            }
        }


        /// <summary>
        /// Averages two values of type T and returns the result as T.
        /// Handles numeric types like int, decimal, double, and long.
        /// </summary>
        /// <typeparam name="T">The numeric type of the values.</typeparam>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>The average of the two values.</returns>
        private static T Average<T>(T a, T b) where T : struct, IComparable<T>, IConvertible
        {
            if (typeof(T) == typeof(int))
            {
                int result = (((int)(object)a + (int)(object)b) / 2);
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(long))
            {
                long result = (((long)(object)a + (long)(object)b) / 2);
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(double))
            {
                double result = (((double)(object)a + (double)(object)b) / 2.0);
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(decimal))
            {
                decimal result = (((decimal)(object)a + (decimal)(object)b) / 2.0m);
                return (T)(object)result;
            }
            else
            {
                throw new NotSupportedException($"Type {typeof(T)} is not supported.");
            }
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
        /// Sorts a collection based on the order of a reference collection using a custom comparison predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
        /// <typeparam name="TOrder">The type of elements in the reference collection.</typeparam>
        /// <param name="source">The source collection to be sorted.</param>
        /// <param name="orderReference">The reference collection that defines the desired order.</param>
        /// <param name="comparisonPredicate">A custom comparison predicate that takes elements from both collections and returns a boolean indicating their relative order.</param>
        /// <returns>A new IEnumerable&lt;TSource&gt; sorted based on the custom comparison predicate.</returns>
        /// <remarks>
        /// <para>
        /// This method sorts the source collection based on the order of elements in the reference collection.
        /// </para>
        /// <para>
        /// It allows you to specify a custom comparison predicate that determines the sorting order based on elements from both collections.
        /// </para>
        /// <para>
        /// If a key in the source collection cannot be found in the orderReference collection, it is placed at the end of the sorted result.
        /// </para>
        /// </remarks>
        public static IEnumerable<TSource> SortByReference<TSource, TOrder>(
            this IEnumerable<TSource> source,
            IEnumerable<TOrder> orderReference,
            Func<TSource, TOrder, bool> comparisonPredicate)
        {
            var indexedSource = source.Select((item, index) => new { Item = item, Index = index });

            // Sort the indexed source based on the orderReference
            var sorted = indexedSource
                .OrderBy(entry =>
                {
                    var sourceItem = entry.Item;
                    var orderIndex = orderReference
                        .Select((item, index) => new { Item = item, Index = index })
                        .FirstOrDefault(referenceItem => comparisonPredicate(sourceItem, referenceItem.Item))?
                        .Index ?? int.MaxValue;
                    return orderIndex;
                })
                .Select(entry => entry.Item);

            return sorted;
        }

        private static bool OrderReferenceComparison<TSource, TOrder>(
            this TSource sourceItem,
            IEnumerable<TOrder> orderReference,
            Func<TSource, TOrder, bool> comparisonPredicate)
        {
            return orderReference.Any(orderItem => comparisonPredicate(sourceItem, orderItem));
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
        /// Filters items from a source collection based on a comparison predicate against a reference collection.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
        /// <typeparam name="TOrder">The type of elements in the reference collection.</typeparam>
        /// <param name="source">The source collection from which to filter items.</param>
        /// <param name="orderReference">The reference collection used for comparison.</param>
        /// <param name="comparisonPredicate">A predicate that determines whether an item in the source collection should be included based on the reference collection.</param>
        /// <returns>An IEnumerable&lt;TSource&gt; containing items from the source collection that satisfy the comparison predicate.</returns>
        /// <remarks>
        /// This method filters items from the source collection based on a comparison predicate against a reference collection.
        /// The comparisonPredicate function is used to determine if an item should be included in the result.
        /// </remarks>
        public static IEnumerable<TSource> FilterByReference<TSource, TOrder>(
            this IEnumerable<TSource> source,
            IEnumerable<TOrder> orderReference,
            Func<TSource, TOrder, bool> comparisonPredicate)
        {
            // Use the comparisonPredicate to determine if an item should be included
            return source.Where(item => orderReference.Any(orderItem => comparisonPredicate(item, orderItem)));
        }
        /// <summary>
        /// Removes duplicate items from a collection based on a comparison predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
        /// <param name="source">The source collection to remove duplicates from.</param>
        /// <param name="comparisonPredicate">A predicate to determine duplicate items.</param>
        /// <returns>An IEnumerable&lt;TSource&gt; containing unique elements based on the comparison predicate.</returns>
        public static IEnumerable<TSource> RemoveDuplicates<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, bool> comparisonPredicate)
        {
            var distinctItems = new List<TSource>();

            foreach (var item in source)
            {
                if (!distinctItems.Any(existingItem => comparisonPredicate(existingItem, item)))
                {
                    distinctItems.Add(item);
                }
            }

            return distinctItems;
        }

        private class LambdaEqualityComparer<TSource> : IEqualityComparer<TSource>
        {
            private readonly Func<TSource, TSource, bool> _equalsFunc;
            private readonly Func<TSource, int> _getHashCodeFunc;

            public LambdaEqualityComparer(Func<TSource, TSource, bool> equalsFunc)
            {
                _equalsFunc = equalsFunc;
                _getHashCodeFunc = obj => obj.GetHashCode();
            }

            public bool Equals(TSource x, TSource y)
            {
                return _equalsFunc(x, y);
            }

            public int GetHashCode(TSource obj)
            {
                return _getHashCodeFunc(obj);
            }
        }

    }
}