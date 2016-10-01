using System.Collections.Generic;
using System.Linq;

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
        /// Gets the Variables associated with the Model.
        /// </summary>
        protected virtual IEnumerable<IntVar> Variables
        {
            get { yield break; }
        }

        /// <summary>
        /// Returns a set of created <see cref="SearchMonitor"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual IEnumerable<SearchMonitor> CreateSearchMonitors(Solver solver, params IntVar[] variables)
        {
            var monitor = solver.MakeAllSolutionCollector();
            foreach (var variable in variables) monitor.Add(variable);
            yield return monitor;
        }

        /// <summary>
        /// Returns a created <see cref="SearchMonitorVector"/> given the
        /// <paramref name="solver"/> and <paramref name="variables"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="vector"></param>
        /// <param name="collector"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual bool TryCreateSearchMonitorVector(Solver solver, out SearchMonitorVector vector,
            out SolutionCollector collector, params IntVar[] variables)
        {
            collector = null;

            vector = new SearchMonitorVector();

            foreach (var monitor in CreateSearchMonitors(solver, variables))
            {
                vector.Add(monitor);

                // One of the monitors should (SHOULD) be a SolutionCollector.
                if (!(monitor is SolutionCollector)) continue;

                collector = (SolutionCollector) monitor;
            }

            ClrCreatedObjects.Add(vector);

            return collector != null;
        }

        /// <summary>
        /// Tries to make a <see cref="DecisionBuilder"/> <paramref name="builder"/> given the
        /// <paramref name="variables"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="builder"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected abstract bool TryMakeDecisionBuilder(Solver solver, out DecisionBuilder builder,
            params IntVar[] variables);

        /// <summary>
        /// Should return true or false, whether or not the <paramref name="assignment"/> was received.
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected abstract bool TryReceiveAssignment(Assignment assignment);

        /// <summary>
        /// Gets the <see cref="OptimizeVar"/> instances from the
        /// <see cref="ProblemSolverBase.ClrCreatedObjects "/>.
        /// </summary>
        protected IEnumerable<OptimizeVar> Optimizations
        {
            get { return ClrCreatedObjects.ToArray().OfType<OptimizeVar>(); }
        }

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

                var variables = Variables.ToArray();

                SearchMonitorVector monitors;
                SolutionCollector collector;

                if (!TryCreateSearchMonitorVector(solver, out monitors, out collector, variables))
                    return false;

                DecisionBuilder builder;

                if (!TryMakeDecisionBuilder(solver, out builder, variables))
                    return false;

                var collection = new ReadOnlyAssignmentCollection(collector);

                var optimizations = Optimizations.ToArray();

                solver.NewSearch(builder, monitors);

                while (solver.NextSolution())
                {
                    // Discard the Solution when it was Not Optimized.
                    if (optimizations.Any() && optimizations.Any(op => !op.AcceptSolution()))
                    {
                        continue;
                    }

                    var assignment = collection[collection.Count - 1];

                    if (TryReceiveAssignment(assignment))
                    {
                        break;
                    }
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
