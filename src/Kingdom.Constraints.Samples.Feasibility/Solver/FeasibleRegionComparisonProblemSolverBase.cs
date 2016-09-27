using System;

namespace Kingdom.Constraints.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;

    public abstract class FeasibleRegionComparisonProblemSolverBase<TProblemSolver>
        : OrLinearProblemSolverBase<TProblemSolver, double>
        where TProblemSolver : FeasibleRegionComparisonProblemSolverBase<TProblemSolver>
    {
        // ???? p => p.o.Value()
        // Apparently not testing this: p => p.x.SolutionValue() + 7d*p.y.SolutionValue() = 17 (integer)
        protected FeasibleRegionComparisonProblemSolverBase(OptimizationProblemType problemType)
            : base(@"Feasible Integer Region", p => p.o.Value(), problemType)
        {
        }

        protected override void PrepareConstraints(Solver solver)
        {
            // x + 7y <= 17.5
            var c1 = solver.MakeConstraint(NegativeInfinity, 17.5);
            ClrCreatedObjects.Add(SetProblemComponent(c1, (p, c) => p.c1 = c));
            c1.SetCoefficient(Problem.x, 1d);
            c1.SetCoefficient(Problem.y, 7d);

            // x <= 3.5
            var c2 = solver.MakeConstraint(NegativeInfinity, 3.5d);
            ClrCreatedObjects.Add(SetProblemComponent(c2, (p, c) => p.c2 = c));
            c2.SetCoefficient(Problem.x, 1d);
            c2.SetCoefficient(Problem.y, 0d);
        }

        protected override void PrepareObjective(Solver solver)
        {
            // Maximize x + 10y
            var obj = solver.Objective();
            ClrCreatedObjects.Add(SetProblemComponent(obj, (p, o) => p.o = o));

            /* The SetCoefficient method sets the coefficients of the objective function.
             * The SetMaximization method makes this a maximization problem. */
            obj.SetCoefficient(Problem.x, 1d);
            obj.SetCoefficient(Problem.y, 10d);
            obj.SetMaximization();
        }

        public class SolutionEventArgs : EventArgs
        {
            public int VariableCount { get; private set; }

            public int ConstraintCount { get; private set; }

            public LinearResultStatus ResultStatus { get; private set; }

            // TODO: TBD: not sure it makes sense to always include a solution...
            public double Solution { get; private set; }

            public dynamic SolutionValues { get; private set; }

            internal SolutionEventArgs(int variableCount, int constraintCount, LinearResultStatus resultStatus,
                double solution, dynamic solutionValues)
            {
                VariableCount = variableCount;
                ConstraintCount = constraintCount;
                ResultStatus = resultStatus;
                Solution = solution;
                SolutionValues = solutionValues;
            }
        }

        public event EventHandler<SolutionEventArgs> Solved;

        private void RaiseSolved(SolutionEventArgs e)
        {
            if (Solved == null) return;
            Solved(this, e);
        }

        protected override void ReceiveSolution(Solver solver, LinearResultStatus resultStatus,
            double solution, dynamic problem)
        {
            var e = new SolutionEventArgs(solver.NumVariables(), solver.NumConstraints(),
                resultStatus, solution, GetSolutionValues(Problem));

            RaiseSolved(e);
        }
    }
}
