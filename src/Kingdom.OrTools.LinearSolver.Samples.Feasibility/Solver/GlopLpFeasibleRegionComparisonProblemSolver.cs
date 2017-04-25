namespace Kingdom.OrTools.LinearSolver.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;
    using static OptimizationProblemType;

    public class GlopLpFeasibleRegionComparisonProblemSolver
        : FeasibleRegionComparisonProblemSolverBase<
            GlopLpFeasibleRegionComparisonProblemSolver>
    {
        public GlopLpFeasibleRegionComparisonProblemSolver()
            : base(GlopLinearProgramming)
        {
        }

        protected override void PrepareVariables(Solver solver)
        {
            // TODO: TBD: ths framework for this solver may change along similar lines as for Constraint Problem Solver.
            // x and y are non-negative integer variables
            var xVar = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "x");
            SetProblemComponent(xVar, (p, x) => p.x = x).TrackClrObject(this);

            var yVar = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "y");
            SetProblemComponent(yVar, (p, y) => p.y = y).TrackClrObject(this);
        }
    }
}
