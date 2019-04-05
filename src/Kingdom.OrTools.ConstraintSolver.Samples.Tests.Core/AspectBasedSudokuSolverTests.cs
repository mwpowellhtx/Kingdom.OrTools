namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using OrTools.Samples;
    using Sudoku;
    using Xunit;
    using Xunit.Abstractions;

    /// <inheritdoc />
    public class AspectBasedSudokuSolverTests : TestFixtureBase
    {
        /// <summary>
        /// Public Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <inheritdoc />
        public AspectBasedSudokuSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies that the aspect yields a solution.
        /// </summary>
        [Fact]
        public void Verify_that_aspect_yields_solution()
        {
            using (var ps = new AspectBasedSudokuProblemSolver("Sudoku Problem Solver"))
            {
                Assert.True(ps.TryResolve());
                Assert.NotNull(ps.Solution);
                ps.Solution.PrettyPrint(OutputHelper.WriteLine);
            }
        }
    }
}
