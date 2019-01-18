using System.Collections.Generic;

namespace Vostok.Configuration.Abstractions.Extensions
{
    internal static class CollectionExtensions
    {
        public static bool SetEquals<T>(this ICollection<T> @this, ICollection<T> other, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(@this, comparer ?? EqualityComparer<T>.Default).SetEquals(other);
        }
    }
}