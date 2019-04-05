using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.LinearSolver.Samples.Feasibility
{
    using Google.OrTools.LinearSolver;
    using static LinearResultStatus;

    /// <summary>
    /// Based on the Feasible Region example provided on the Google Optimization Linear
    /// Programming (LP) web site.
    /// </summary>
    /// <see cref="!:http://developers.google.com/optimization/lp/glop" />
    /// <inheritdoc />
    public class FeasibleRegionProblemSolverWithoutSolution
        : OrLinearProblemSolverBase<FeasibleRegionProblemSolverWithoutSolution>
    {
        /// <inheritdoc />
        public FeasibleRegionProblemSolverWithoutSolution()
            : base(@"Feasible Region")
        {
        }

        private IEnumerable<Variable> _variables;

        protected override IEnumerable<Variable> Variables
        {
            get
            {
                IEnumerable<Variable> GetAll()
                {
                    var solver = Solver;

                    var x = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "x");
                    yield return SetProblemComponent(x, (p, m) => p.x = m);

                    var y = solver.MakeNumVar(NegativeInfinity, PositiveInfinity, "y");
                    yield return SetProblemComponent(y, (p, m) => p.y = m);
                }

                return _variables ?? (_variables = GetAll().ToArray());
            }
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

        protected override void ReceiveSolution(Solver solver, LinearResultStatus resultStatus, dynamic problem)
        {
            var solution = resultStatus == Optimal || resultStatus == Feasible;
            var e = new SolutionEventArgs(solver, resultStatus, solution, GetSolutionValues(problem));
            OnSolved(e);
        }
    }
}
