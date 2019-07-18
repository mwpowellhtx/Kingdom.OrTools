using System.Collections;
using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit.Abstractions;

    public abstract class TestFixtureBase
    {
        protected ITestOutputHelper OutputHelper { get; }

        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        /// <summary>
        /// Returns the Range of <paramref name="values"/> as a fresh <see cref="IEnumerable{T}"/>
        /// instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static IEnumerable<T> GetRange<T>(params T[] values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in values)
            {
                yield return x;
            }
        }

        /// <summary>
        /// Enumerates the weakly typed <paramref name="values"/> to a Sequence of
        /// <see cref="object"/>.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected static IEnumerable<object> Enumerate(IEnumerable values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in values)
            {
                yield return x;
            }
        }
    }
}
