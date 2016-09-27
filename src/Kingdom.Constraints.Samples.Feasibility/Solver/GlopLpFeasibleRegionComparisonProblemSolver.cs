namespace Kingdom.Constraints.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;

    public class GlopLpFeasibleRegionComparisonProblemSolver
        : FeasibleRegionComparisonProblemSolverBase<
            GlopLpFeasibleRegionComparisonProblemSolver>
    {
        public GlopLpFeasibleRegionComparisonProblemSolver()
            : base(OptimizationProblemType.GlopLinearProgramming)
        {
        }

        protected override void PrepareVariables(Solver solver)
        {
            // x and y are non-negative integer variables
            var xVar = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "x");
            ClrCreatedObjects.Add(SetProblemComponent(xVar, (p, x) => p.x = x));

            var yVar = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "y");
            ClrCreatedObjects.Add(SetProblemComponent(yVar, (p, y) => p.y = y));
        }
    }
}
