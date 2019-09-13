using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    /// <summary>
    /// Provides a set of helpful <see cref="ICollection{T}"/> extension methods.
    /// </summary>
    internal static class CollectionExtensionMethods
    {
        /// <summary>
        /// Adds the Range of <paramref name="values"/> to the <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static ICollection<T> AddRange<T>(this ICollection<T> collection, params T[] values)
        {
            foreach (var x in values)
            {
                collection.Add(x);
            }

            return collection;
        }
    }
}
