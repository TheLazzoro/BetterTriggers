using System;
using System.Collections.Generic;

public static class ObservableCollectionExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var cur in enumerable)
        {
            action(cur);
        }
    }
}
