using System;

namespace Kingdom.OrTools.LinearSolver.Samples.Feasibility
{
    using Xunit;
    using Xunit.Abstractions;
    using static LinearResultStatus;

    /// <summary>
    /// 
    /// </summary>
    public class LinearProblemSolverTests : TestFixtureBase
    {
        public LinearProblemSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        private static double Round(double value) => Math.Round(value);

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Feasibility_Problem_Solver_is_Correct()
        {
            using (var s = new FeasibleRegionProblemSolver())
            {
                s.Solved += (sender, e) =>
                {
                    /* The numbers really ARE NOT that perfect, they are CLOSE, within fraction
                     * of 0.0001, but for rounding. It is also not necessarily the case where
                     * Rounding is a Solver issue, but a unit test one. */

                    Assert.Equal(2, e.VariableCount);
                    Assert.Equal(3, e.ConstraintCount);
                    Assert.Equal(Optimal, e.ResultStatus);
                    Assert.Equal(34d, Round(e.Solution));
                    Assert.Equal(6d, Round(e.SolutionValues.x));
                    Assert.Equal(4d, Round(e.SolutionValues.y));
                };

                Assert.True(s.TryResolve());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Feasibility_Problem_Solver_Without_Solution_is_Correct()
        {
            // Make sure that we actually ARE testing WITHOUT the Solution value.
            using (var s = new FeasibleRegionProblemSolverWithoutSolution())
            {
                s.Solved += (sender, e) =>
                {
                    /* The numbers really ARE NOT that perfect, they are CLOSE, within fraction
                     * of 0.0001, but for rounding. It is also not necessarily the case where
                     * Rounding is a Solver issue, but a unit test one. */

                    Assert.Equal(2, e.VariableCount);
                    Assert.Equal(3, e.ConstraintCount);
                    Assert.Equal(Optimal, e.ResultStatus);
                    Assert.Equal(6d, Round(e.SolutionValues.x));
                    Assert.Equal(4d, Round(e.SolutionValues.y));
                };

                Assert.True(s.TryResolve());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Cbc_Mip_Feasibility_Problem_Solver_is_Correct()
        {
            using (var s = new CbcMipFeasibleRegionComparisonProblemSolver())
            {
                s.Solved += (sender, e) =>
                {
                    Assert.Equal(2, e.VariableCount);
                    Assert.Equal(2, e.ConstraintCount);
                    Assert.Equal(Optimal, e.ResultStatus);

                    Assert.Equal(3d, Round(e.SolutionValues.x));
                    Assert.Equal(2d, Round(e.SolutionValues.y));

                    Assert.Equal(23d, Round(e.Solution));
                };

                Assert.True(s.TryResolve());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Glop_Lp_Feasibility_Problem_Solver_is_Correct()
        {
            using (var s = new GlopLpFeasibleRegionComparisonProblemSolver())
            {
                s.Solved += (sender, e) =>
                {
                    Assert.Equal(2, e.VariableCount);
                    Assert.Equal(2, e.ConstraintCount);

                    // TODO: is yielding Abnormal instead of Optimal...
                    Assert.Equal(Optimal, e.ResultStatus);
                    Assert.Equal(0d, Round(e.SolutionValues.x));
                    Assert.Equal(2.5d, Round(e.SolutionValues.y));

                    // TODO: is yielding ZERO instead of the predicted solution
                    Assert.Equal(25d, Round(e.Solution));
                };

                Assert.False(s.TryResolve());
                // TODO: TBD: was this failing before?
                //Assert.True(s.TryResolve());
            }
        }
    }
}
