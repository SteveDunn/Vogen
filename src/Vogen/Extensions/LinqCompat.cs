using System;
using System.Collections.Generic;

namespace Vogen.Extensions;

internal static class LinqCompat
{
    public static IEnumerable<TSource> DistinctByCompat<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null)
    {
        var seen = new HashSet<TKey>(comparer);

        foreach (var item in source)
        {
            if (seen.Add(keySelector(item)))
            {
                yield return item;
            }
        }
    }
}