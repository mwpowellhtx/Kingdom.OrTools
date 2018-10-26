namespace Kingdom.OrTools.LinearSolver.Samples.Feasibility
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
            var c1 = solver.MakeConstraint(NegativeInfinity, 17.5d);
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
    }
}
