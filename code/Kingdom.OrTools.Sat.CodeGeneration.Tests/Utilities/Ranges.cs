using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    internal static class Ranges
    {
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> Range corresponding to the
        /// <paramref name="values"/>. It is important that we return a fresh instance of
        /// <see cref="IEnumerable{T}"/> and not the <paramref name="values"/> itself
        /// because that may have been a collection other than that, an array, and so on.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetRange<T>(params T[] values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in values)
            {
                // Yes, we want to Yield Return them individually.
                yield return x;
            }
            // Yes, we do not want to return the given Values instance itself.
        }
    }
}
