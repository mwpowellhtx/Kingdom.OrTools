namespace Kingdom.Constraints.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;

    public class CbcMipFeasibleRegionComparisonProblemSolver
        : FeasibleRegionComparisonProblemSolverBase<
            CbcMipFeasibleRegionComparisonProblemSolver>
    {
        public CbcMipFeasibleRegionComparisonProblemSolver()
            : base(OptimizationProblemType.CbcMixedIntegerProgramming)
        {
        }

        protected override void PrepareVariables(Solver solver)
        {
            // x and y are non-negative integer variables
            var xVar = solver.MakeIntVar(0d, PositiveInfinity, "x");
            ClrCreatedObjects.Add(SetProblemComponent(xVar, (p, x) => p.x = x));

            var yVar = solver.MakeIntVar(0d, PositiveInfinity, "y");
            ClrCreatedObjects.Add(SetProblemComponent(yVar, (p, y) => p.y = y));
        }

        protected override bool VerifySolution(Solver solver, LinearResultStatus resultStatus)
        {
            /* The solution looks legit; however, when using solvers other than GLOP_LINEAR_PROGRAMMING,
             * verifying the solution is highly recommended! */
            return base.VerifySolution(solver, resultStatus) && solver.VerifySolution(1e-7, true);
        }
    }
}
