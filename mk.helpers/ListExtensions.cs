using System;
using System.Collections.Generic;
using System.Linq;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for working with lists and collections.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Batches the elements of a sequence into smaller sequences of a specified maximum size.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="items">The sequence to be batched.</param>
        /// <param name="maxItems">The maximum number of items in each batch.</param>
        /// <returns>An IEnumerable of batches containing items from the input sequence.</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                        .GroupBy(x => x.inx / maxItems)
                        .Select(g => g.Select(x => x.item));
        }


        /// <summary>
        /// Batches the elements of an IQueryable collection into smaller sequences of a specified size.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The IQueryable collection to be batched.</param>
        /// <param name="size">The size of each batch.</param>
        /// <returns>An IEnumerable of IQueryable batches from the input collection.</returns>
        public static IEnumerable<IQueryable<T>> Batch<T>(this IQueryable<T> collection, int size)
        {
            var totalSize = collection.Count();
            for (var start = 0; start < totalSize; start += size)
            {
                yield return collection.Skip(start).Take(size);
            }
        }
        /// <summary>
        /// Adds a range of items to the end of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to which items are added.</param>
        /// <param name="items">The items to be added.</param>
        public static void AddRange<T>(this List<T> list, params T[] items)
        => list.AddRange(items);


        /// <summary>
        /// Removes null elements from the IEnumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the IEnumerable.</typeparam>
        /// <param name="items">The IEnumerable containing elements to be filtered.</param>
        /// <returns>An IEnumerable with null elements removed.</returns>
        public static IEnumerable<T> Denullify<T>(this IEnumerable<T> items)
        => items.Where(s => s != null);


        /// <summary>
        /// Generates all permutations of the elements in the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list for which permutations are generated.</param>
        /// <returns>A List of Lists representing all permutations of the list's elements.</returns>
        public static List<List<T>> Permutations<T>(this List<T> list)
            => GetPermutations(list, list.Count).Select(d => d.ToList()).ToList();


        /// <summary>
        /// Generates permutations of a specified length from the elements in the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list for which permutations are generated.</param>
        /// <param name="length">The length of permutations to generate.</param>
        /// <returns>A List of Lists representing permutations of the specified length.</returns>
        public static List<List<T>> Permutations<T>(this List<T> list, int length)
        => GetPermutations(list, length).Select(d => d.ToList()).ToList();

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
        {
            if (list == null) return null;
            if (length == 1) return list.Select(t => new[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new[] { t2 }));
        }
        /// <summary>
        /// Generates all combinations of the elements in the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list for which combinations are generated.</param>
        /// <returns>A List of Lists representing all combinations of the list's elements.</returns>
        public static List<List<T>> Combinations<T>(this List<T> list)
        {
            var comboCount = (int)Math.Pow(2, list.Count) - 1;
            var result = new List<List<T>>();
            for (var i = 1; i < comboCount + 1; i++)
            {
                result.Add(new List<T>());
                for (var j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }
            return result;
        }

    }
}
