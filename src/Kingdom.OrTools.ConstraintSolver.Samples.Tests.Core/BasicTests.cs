using System;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using Google.OrTools.ConstraintSolver;
    using OrTools.Samples;
    using Xunit;
    using Xunit.Abstractions;
    using static Math;

    /// <summary>
    /// Performs some basic unit testing.
    /// </summary>
    /// <inheritdoc />
    public class BasicTests : TestFixtureBase
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <inheritdoc />
        public BasicTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies that the Flatten functionality works as expected.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory
         , InlineData(0, 10)
         , InlineData(10, 0)
         , InlineData(10, 10)]
        public void VerifyThatFlattenWorksCorrectly(int x, int y)
        {
            // Assumes that at least one dimension actually has dimension.
            Assert.True(x >= 0);
            Assert.True(y >= 0);
            Assert.True(x + y > 0);

            var xMax = Max(x, 1);
            var yMax = Max(y, 1);

            var arr = new IntVar[xMax, yMax];

            Assert.Equal(xMax, arr.GetLength(0));
            Assert.Equal(yMax, arr.GetLength(1));

            var flattened = arr.Flatten();

            Assert.Equal(xMax * yMax, flattened.Length);
        }
    }
}
