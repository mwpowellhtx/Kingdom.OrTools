using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Variable> GetVariables(Solver solver)
        {
            // TODO: TBD: ths framework for this solver may change along similar lines as for Constraint Problem Solver.
            // x and y are non-negative integer variables
            var xVar = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "x");
            yield return SetProblemComponent(xVar, (p, x) => p.x = x).TrackClrObject(this);

            var yVar = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "y");
            yield return SetProblemComponent(yVar, (p, y) => p.y = y).TrackClrObject(this);
        }
    }
}
