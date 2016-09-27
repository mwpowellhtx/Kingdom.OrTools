namespace Kingdom.Constraints
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// or-tools-based Constraint Programming problem solver.
    /// </summary>
    /// <see cref="!:http://code.google.com/p/or-tools/"/>
    public abstract class OrProblemSolverBase<TProblemSolver>
        : ProblemSolverBase, IOrProblemSolver<TProblemSolver>
        where TProblemSolver : OrProblemSolverBase<TProblemSolver>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modelName"></param>
        protected OrProblemSolverBase(string modelName)
            : base(modelName)
        {
        }

        /// <summary>
        /// Returns a seed for the <see cref="Solver"/> to <see cref="Solver.ReSeed"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract int GetSolverSeed();

        /// <summary>
        /// Initializes the problem solver.
        /// </summary>
        /// <param name="solver"></param>
        protected virtual void Initialize(Solver solver)
        {
            var seed = GetSolverSeed();

            solver.ReSeed(seed);
        }

        /// <summary>
        /// Prepares solver variables.
        /// </summary>
        /// <param name="solver"></param>
        protected abstract void PrepareVariables(Solver solver);

        /// <summary>
        /// Prepares the solver constraints.
        /// </summary>
        /// <param name="solver"></param>
        protected abstract void PrepareConstraints(Solver solver);

        /// <summary>
        /// Prepares the solver solution collector.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="collector"></param>
        protected abstract void PrepareSolutionCollector(Solver solver, SolutionCollector collector);

        /// <summary>
        /// Prepares any solver monitors that should watch.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="monitors"></param>
        protected virtual void PrepareMonitors(Solver solver, SearchMonitorVector monitors)
        {
            ////TODO: ? time limits ? or arrest the constraints themselves?
            //var searchLimit = solver.MakeLimit(3000, 100, 100, 100);
            //monitors.Insert(0, searchLimit);
        }

        /// <summary>
        /// Tries to make a decision buidler.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected abstract bool TryMakeDecisionBuilder(Solver solver, out DecisionBuilder builder);

        /// <summary>
        /// Should return true or false, whether or not the <paramref name="assignment"/> was received.
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected abstract bool TryReceiveAssignment(Assignment assignment);

        /// <summary>
        /// Tries to Resolve the problem.
        /// </summary>
        /// <returns></returns>
        public override bool TryResolve()
        {
            using (var solver = new Solver(ModelName))
            {
                Initialize(solver);

                PrepareVariables(solver);
                PrepareConstraints(solver);

                //TODO: TBD: whether this isn't the better choice? solver.MakeAllSolutionCollector()
                //TODO: but then, what to consider "owner" of monitor(s)?
                //TODO: TBD: Solver may be the owner of the vector itself, which we add a "Made" SolutionCollector: but this is not obvious
                var solutionCollector = solver.MakeAllSolutionCollector();
                var monitors = new SearchMonitorVector {solutionCollector};

                PrepareMonitors(solver, monitors);
                PrepareSolutionCollector(solver, solutionCollector);

                DecisionBuilder db;

                if (!TryMakeDecisionBuilder(solver, out db))
                    return false;

                var collection = new ReadOnlyAssignmentCollection(solutionCollector);

                solver.NewSearch(db, monitors);

                while (solver.NextSolution())
                {
                    var assignment = collection[collection.Count - 1];

                    if (TryReceiveAssignment(assignment)) break;
                }

                solver.EndSearch();
            }
            return true;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
