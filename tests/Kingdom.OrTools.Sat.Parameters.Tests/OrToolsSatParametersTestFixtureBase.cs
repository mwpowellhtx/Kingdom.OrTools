using System.Reflection;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit;
    using Xunit.Abstractions;

    public abstract class OrToolsSatParametersTestFixtureBase : TestFixtureBase
    {
        /// <summary>
        /// Gets the ParametersAssembly for use throughout the unit tests.
        /// </summary>
        protected static Assembly ParametersAssembly { get; } = typeof(Anchor).Assembly;

        protected OrToolsSatParametersTestFixtureBase(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verify that we actually have an <see cref="ParametersAssembly"/> instance.
        /// </summary>
        [Fact]
        public void Has_Parameters_Assembly() => ParametersAssembly.AssertNotNull();
    }
}
