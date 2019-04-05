using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.LinearSolver.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;
    using static LinearOptimizationProblemType;

    public class CbcMipFeasibleRegionComparisonProblemSolver
        : FeasibleRegionComparisonProblemSolverBase<
            CbcMipFeasibleRegionComparisonProblemSolver>
    {
        public CbcMipFeasibleRegionComparisonProblemSolver()
            : base(CbcMixedIntegerProgramming)
        {
        }

        private IEnumerable<Variable> _variables;

        protected sealed override IEnumerable<Variable> Variables
        {
            get
            {
                IEnumerable<Variable> GetAll()
                {
                    var solver = Solver;

                    // x and y are non-negative integer variables
                    var xVar = solver.MakeIntVar(0d, PositiveInfinity, "x");
                    yield return SetProblemComponent(xVar, (p, x) => p.x = x);

                    var yVar = solver.MakeIntVar(0d, PositiveInfinity, "y");
                    yield return SetProblemComponent(yVar, (p, y) => p.y = y);
                }

                return _variables ?? (_variables = GetAll().ToArray());
            }
        }

        protected override bool VerifySolution(Solver solver, LinearResultStatus resultStatus)
        {
            /* The solution looks legit; however, when using solvers other than GLOP_LINEAR_PROGRAMMING,
             * verifying the solution is highly recommended! */
            return base.VerifySolution(solver, resultStatus) && solver.VerifySolution(1e-7, true);
        }
    }
}
