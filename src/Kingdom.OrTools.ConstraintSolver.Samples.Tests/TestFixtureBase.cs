using System.Collections.Generic;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using Xunit.Abstractions;

    public abstract class TestFixtureBase
    {
        protected static IEnumerable<T> GetRange<T>(params T[] values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in values)
            {
                yield return x;
            }
        }

        protected ITestOutputHelper OutputHelper { get; }

        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }
    }
}
