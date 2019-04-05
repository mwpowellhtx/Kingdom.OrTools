namespace Kingdom.OrTools.LinearSolver.Samples.Feasibility
{
    using Xunit.Abstractions;

    public abstract class TestFixtureBase
    {
        protected ITestOutputHelper OutputHelper { get; }

        protected TestFixtureBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }
    }
}
