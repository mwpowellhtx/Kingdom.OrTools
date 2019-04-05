using System.Collections;
using System.Collections.Generic;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Performs some basic unit testing.
    /// </summary>
    /// <inheritdoc />
    public abstract class TestCasesBase : IEnumerable<object[]>
    {
        protected static IEnumerable<T> GetRange<T>(params T[] values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in values)
            {
                yield return x;
            }
        }

        protected abstract IEnumerable<object[]> Cases { get; }

        public IEnumerator<object[]> GetEnumerator() => Cases.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
