using System;
using System.Collections.Generic;

namespace SafeBunny.Core.Extensions
{
    internal static class ListExtensions
    {
        internal static void Execute<T>(this List<T> list, Action<List<T>> act) => act(list);
    }
}