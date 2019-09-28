using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    // TODO: TBD: these functions could actually serve pretty well in our Collections package(s) ...
    internal static class RangeExtensionMethods
    {
        /// <summary>
        /// Returns the <see cref="IEnumerable{T}" /> Range corresponding to the
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> MakeRange<T>(this T value)
        {
            yield return value;
        }

        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> Range corresponding to the
        /// <paramref name="value"/> and any <paramref name="others"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> MakeRangeWith<T>(this T value, params T[] others)
        {
            yield return value;

            foreach (var x in others)
            {
                yield return x;
            }
        }

        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> Range corresponding to the
        /// <paramref name="values"/> and any <paramref name="others"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<T> MakeRangeWith<T>(this IEnumerable<T> values, params T[] others)
        {
            foreach (var x in values)
            {
                yield return x;
            }

            foreach (var x in others)
            {
                yield return x;
            }
        }
    }
}
