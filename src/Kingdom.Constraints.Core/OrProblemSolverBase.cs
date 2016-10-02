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
            ClrCreatedObjects.Add(monitor);
            yield return monitor;
        }

        /// <summary>
        /// Tries to make a <see cref="DecisionBuilder"/> given the  <paramref name="variables"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected abstract DecisionBuilder CreateDecisionBuilder(Solver solver, params IntVar[] variables);

        /// <summary>
        /// Returns whether could Receive the Next <paramref name="assignment"/>.
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns>Whether a Next Solution ought to be obtained for consideration.</returns>
        protected virtual bool TryReceiveNextAssignment(Assignment assignment)
        {
            return false;
        }

        /// <summary>
        /// Returns whether could Receive the Next Solution.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual bool TryReceiveNext(params IntVar[] variables)
        {
            return false;
        }

        /// <summary>
        /// Returns whether the End <paramref name="assignment"/> could be received.
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected abstract bool TryReceiveEndAssignment(Assignment assignment);

        /// <summary>
        /// Returns whether could Receive the End Solution.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual bool TryReceiveEnd(params IntVar[] variables)
        {
            return false;
        }

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

                var monitors = CreateSearchMonitors(solver, variables).ToArray();

                var builder = CreateDecisionBuilder(solver, variables);

                solver.NewSearch(builder, monitors);

                var collection = new ReadOnlyAssignmentCollection(
                    monitors.OfType<SolutionCollector>().SingleOrDefault());

                while (solver.NextSolution())
                {
                    //// TODO: TBD: "Optimizations" do not appear to be necessary here?
                    //// Discard the Solution when it was Not Optimized.
                    //if (optimizations.Any() && optimizations.Any(op => !op.AcceptSolution()))
                    //{
                    //    continue;
                    //}

                    /* Be careful of some LINQ extension methods such as Last or LastOrDefault.
                     * Apparently we may receive an Assignment here, but potentially not the last valid one. */
                    if (collection.HasCollector)
                    {
                        if (collection.Any()
                            && TryReceiveNextAssignment(collection[collection.Count - 1]))
                        {
                            break;
                        }

                        continue;
                    }

                    if (TryReceiveNext(variables))
                    {
                        break;
                    }
                }

                // Receive the End Assignment here.
                var received = (collection.HasCollector
                                && collection.Any()
                                && TryReceiveEndAssignment(collection[collection.Count - 1]))
                               || TryReceiveEnd(variables);

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
