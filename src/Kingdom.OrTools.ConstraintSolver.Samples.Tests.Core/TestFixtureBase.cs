namespace Kingdom.OrTools.Samples
{
    using Xunit.Abstractions;

    /// <summary>
    /// Represents the base class of Unit Tests.
    /// </summary>
    public abstract class TestFixtureBase
    {
        /// <summary>
        /// Gets the OutputHelper.
        /// </summary>
        private ITestOutputHelper OutputHelper { get; }

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
