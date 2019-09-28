using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    internal static class CollectionExtensionMethods
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> values, Action<T> action = null)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var value in values)
            {
                action?.Invoke(value);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            return values;
        }

        /// <summary>
        /// Returns the <paramref name="values"/> delimited by the thing created by
        /// <paramref name="delimiterFactory"/>. This behavior is analog to
        /// <see cref="string.Join(string, IEnumerable{string})"/> or virtually any of its variants.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="delimiterFactory"></param>
        /// <returns></returns>
        public static IEnumerable<T> Join<T>(this IEnumerable<T> values, Func<T> delimiterFactory = null)
        {
            delimiterFactory = delimiterFactory ?? (() => default(T));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!values.Any())
            {
                yield break;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            yield return values.First();

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var value in values.Skip(1))
            {
                yield return delimiterFactory();

                yield return value;
            }
        }
    }
}
