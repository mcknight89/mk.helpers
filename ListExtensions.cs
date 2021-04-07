using System;
using System.Collections.Generic;
using System.Linq;

namespace mk.helpers
{
    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                        .GroupBy(x => x.inx / maxItems)
                        .Select(g => g.Select(x => x.item));
        }
        public static IEnumerable<IQueryable<T>> Batch<T>(this IQueryable<T> collection, int size)
        {
            var totalSize = collection.Count();
            for (var start = 0; start < totalSize; start += size)
            {
                yield return collection.Skip(start).Take(size);
            }
        }

        public static void AddRange<T>(this List<T> list, params T[] items)
        => list.AddRange(items);

        public static IEnumerable<T> Denullify<T>(this IEnumerable<T> items)
        => items.Where(s => s != null);

        public static List<List<T>> Permutations<T>(this List<T> list)
            => GetPermutations(list, list.Count).Select(d => d.ToList()).ToList();

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
