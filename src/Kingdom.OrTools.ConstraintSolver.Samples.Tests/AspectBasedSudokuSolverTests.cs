namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using Sudoku;
    using Xunit;
    using Xunit.Abstractions;

    public class AspectBasedSudokuSolverTests : TestFixtureBase
    {
        public AspectBasedSudokuSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies that the Aspect based Problem Solving Yields a Solution.
        /// </summary>
        [Fact]
        public void Aspect_Based_Problem_Solving_Should_Yield_Solution()
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
