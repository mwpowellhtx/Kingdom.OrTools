using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Xunit.Abstractions;

    public abstract class TestFixtureBase
    {
        protected ITestOutputHelper OutputHelper { get; }

        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        protected static IEnumerable<T> GetRange<T>(params T[] values)
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
