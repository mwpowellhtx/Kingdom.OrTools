using System;
using Xunit.Abstractions;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using Google.OrTools.ConstraintSolver;
    using OrTools.Samples;
    using Xunit;

    /// <inheritdoc />
    public class SerializationTests : TestFixtureBase
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <inheritdoc />
        public SerializationTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

#if false

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Testing()
        {
            const string exportFile = "this is a test";

            using (var s = new Solver("Test"))
            {
                var p1 = s.Parameters();
                Assert.NotNull(p1);

                Assert.Equal(string.Empty, p1.ExportFile);
                Assert.Equal(exportFile, p1.ExportFile = exportFile);

                var p2 = s.Parameters();
                Assert.NotNull(p2);

                // Given the underlying SWIG, I would not expect this to be the case.
                // In other words, two different reference proxies are generated.
                Assert.NotSame(p1, p2);

                // However, I **WOULD** expect that this be the case.
                Assert.Equal(exportFile, p2.ExportFile);
                Assert.Equal(p2.ExportFile, p1.ExportFile);

                // But given the fact that a **COPY** of parameters_ is being returned,
                // what we in fact end up with is no-change, from a language perspective.
            }
        }

#endif

    }
}
