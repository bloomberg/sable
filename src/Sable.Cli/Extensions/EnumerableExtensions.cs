// Copyright 2024 Bloomberg Finance L.P.
// Distributed under the terms of the MIT license.

namespace Sable.Cli.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, bool isInclusive)
    {
        foreach (var entry in enumerable)
        {
            if (predicate(entry))
            {
                yield return entry;
            }
            else
            {
                if (isInclusive)
                {
                    yield return entry;
                }
                yield break;
            }
        }
    }
}
