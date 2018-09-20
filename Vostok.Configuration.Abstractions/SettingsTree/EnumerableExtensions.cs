using System.Collections.Generic;
using System.Linq;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Unique<T>(this IEnumerable<T> source, IEnumerable<T> list, IEqualityComparer<T> comparer = null)
        {
            if (source == null)
                return list;
            if (list == null)
                return source;

            var src = source as T[] ?? source.ToArray();
            var lst = list as T[] ?? list.ToArray();

            var unique1 = src.Except(lst, comparer);
            var unique2 = lst.Except(src, comparer);
            return unique1.Concat(unique2).ToArray();
        }
    }
}