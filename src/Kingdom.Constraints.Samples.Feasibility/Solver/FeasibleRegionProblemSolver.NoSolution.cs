using System;

namespace Kingdom.Constraints.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;

    /// <summary>
    /// Based on the Feasible Region example provided on the Google Optimization Linear
    /// Programming (LP) web site.
    /// </summary>
    /// <see cref="!:http://developers.google.com/optimization/lp/glop" />
    public class FeasibleRegionProblemSolverWithoutSolution
        : OrLinearProblemSolverBase<FeasibleRegionProblemSolverWithoutSolution>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public FeasibleRegionProblemSolverWithoutSolution()
            : base(@"Feasible Region")
        {
        }

        protected override void PrepareVariables(Solver solver)
        {
            var x = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "x");
            var y = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "y");
            ClrCreatedObjects.Add(x);
            ClrCreatedObjects.Add(y);
            SetProblemComponent(x, (p, m) => p.x = m);
            SetProblemComponent(y, (p, m) => p.y = m);
        }

        protected override void PrepareConstraints(Solver solver)
        {
            {
                // x + 2y <= 14
                var c = solver.MakeConstraint(NegativeInfinity, 14);
                c.SetCoefficient(Problem.x, 1);
                c.SetCoefficient(Problem.y, 2);
                ClrCreatedObjects.Add(c);
                SetProblemComponent(c, (p, m) => p.c1 = m);
            }

            {
                // 3x - y >= 0
                var c = solver.MakeConstraint(0, PositiveInfinity);
                c.SetCoefficient(Problem.x, 3);
                c.SetCoefficient(Problem.y, -1);
                ClrCreatedObjects.Add(c);
                SetProblemComponent(c, (p, m) => p.c2 = m);
            }

            {
                // x - y <= 2
                var c = solver.MakeConstraint(NegativeInfinity, 2);
                c.SetCoefficient(Problem.x, 1);
                c.SetCoefficient(Problem.y, -1);
                ClrCreatedObjects.Add(c);
                SetProblemComponent(c, (p, m) => p.c3 = m);
            }
        }

        protected override void PrepareObjective(Solver solver)
        {
            var obj = solver.Objective();
            obj.SetCoefficient(Problem.x, 3);
            obj.SetCoefficient(Problem.y, 4);
            obj.SetMaximization();
            ClrCreatedObjects.Add(obj);
            SetProblemComponent(obj, (p, m) => p.obj = m);
        }

        public class SolutionEventArgs : EventArgs
        {
            public int VariableCount { get; private set; }

            public int ConstraintCount { get; private set; }

            public LinearResultStatus ResultStatus { get; private set; }

            public dynamic SolutionValues { get; private set; }

            internal SolutionEventArgs(int variableCount, int constraintCount, LinearResultStatus resultStatus,
                dynamic solutionValues)
            {
                VariableCount = variableCount;
                ConstraintCount = constraintCount;
                ResultStatus = resultStatus;
                SolutionValues = solutionValues;
            }
        }

        public event EventHandler<SolutionEventArgs> Solved;

        private void RaiseSolved(SolutionEventArgs e)
        {
            if (Solved == null) return;
            Solved(this, e);
        }

        protected override void ReceiveSolution(Solver solver, LinearResultStatus resultStatus, dynamic problem)
        {
            var e = new SolutionEventArgs(solver.NumVariables(), solver.NumConstraints(),
                resultStatus, GetSolutionValues(problem));

            RaiseSolved(e);
        }
    }
}
