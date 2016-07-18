using System;
using System.Collections.Generic;
using System.Linq;

namespace CarRent.DataAccess.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> AddFilter<T>(this IEnumerable<T> collection, Func<T, bool> filter)
        {
            if (filter == null)
            {
                return collection;
            }
            return collection.Where(filter);
        }
    }
}
