using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers
{
    public abstract class PagedResultBase
    {
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }

        public int FirstRowOnPage
        {

            get { return (CurrentPage - 1) * PageSize + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(CurrentPage * PageSize, RowCount); }
        }
    }

    public class PagedResult<T> : PagedResultBase where T : class
    {
        public List<T> Results { get; set; }

        public PagedResult()
        {
            Results = null;
        }
    }


    public static class IQueryableExtensions
    {
        public static PagedResult<T> Paged<T>(this IEnumerable<T> source, int page,  int pageSize) where T : class
        {
            var data = source
              .Skip((page - 1) * pageSize)
              .Take(pageSize).ToList();
            var total = source.Count();
            return new PagedResult<T>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = total,
                PageCount = total/pageSize,
                PageSize = data.Count()
            };

        }

    }

    public static class EnumerableExtensions
    {


        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, T item)
            => sequence?.Concat(item.Yield());

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> sequence, T item)
            => item?.Yield().Concat(sequence);

        public static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }

        public static bool IsAnyOf<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            return list.Contains(source);
        }

    }
}
