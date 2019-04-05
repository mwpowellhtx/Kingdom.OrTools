using System.Collections.Generic;

namespace Kingdom.OrTools.Samples
{
    using Xunit.Abstractions;

    /// <summary>
    /// Represents the base class of Unit Tests.
    /// </summary>
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

        /// <summary>
        /// Gets the OutputHelper.
        /// </summary>
        protected ITestOutputHelper OutputHelper { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        /// <summary>
        /// Shows the <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        protected void ShowMessage(string message, params object[] args)
            => OutputHelper.WriteLine(message, args);
    }
}
