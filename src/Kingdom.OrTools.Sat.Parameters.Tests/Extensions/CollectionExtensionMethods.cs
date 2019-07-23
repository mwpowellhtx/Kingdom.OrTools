using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    internal static class CollectionExtensionMethods
    {
        // ReSharper disable PossibleMultipleEnumeration
        /// <summary>
        /// Permutations are overkill. This approach is quite perfectly adequate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="k"></param>
        /// <param name="slice"></param>
        /// <param name="remaining"></param>
        /// <returns></returns>
        public static bool TrySliceOverCollection<T>(this IEnumerable<T> values, int k, out IEnumerable<T> slice, out IEnumerable<T> remaining)
        {
            var x = values.Count() / k;
            // Default to Values and Empty Array when there are effectively Fewer Remaining.
            slice = (x > 0 ? values.Where((y, i) => i % x == 0) : values).ToArray();
            remaining = (x > 0 ? values.Where((y, i) => i % x != 0) : Array.Empty<T>()).ToArray();
            return slice.Any();
        }
        // ReSharper restore PossibleMultipleEnumeration
    }
}
