using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    // TODO: TBD: consider a fluent configuration-based approach...

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
        protected virtual void ReSeed(Solver solver)
        {
            var seed = GetSolverSeed();

            solver.ReSeed(seed);
        }

        /* TODO: TBD: otherwise "free-standing" calls like PrepareVariables PrepareConstraints or
         * even ReSeed might be better exposed via an exposed fluent configuration... */
         
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

        // TODO: TBD: PrepareSearchMonitors is perhaps closer to a fluent style...

        /// <summary>
        /// Override to prepare the <see cref="SearchMonitor"/> or monitors for the
        /// <paramref name="agent"/> and corresponding <see cref="Solver"/>. If called, the base
        /// method should be called after preparing any other required Monitors. By default, a
        /// single <see cref="SolutionCollector"/> is prepared.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual ISearchAgent PrepareSearchMonitors(ISearchAgent agent, params IntVar[] variables)
        {
            if (!agent.HasSolutionCollector<SolutionCollector>())
            {
                // Start with a single all-solution-collector.
                agent.Monitor(a =>
                {
                    var m = a.Solver.MakeAllSolutionCollector();
                    foreach (var v in variables) m.Add(v);
                    return m;
                });
            }

            return agent;
        }

        /// <summary>
        /// Prepares the <see cref="ISearchAgent"/> corresponding with the
        /// <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        protected virtual ISearchAgent PrepareSearch(Solver solver, params IntVar[] variables)
        {
            var a = solver.PrepareSearch(variables);
            PrepareSearchMonitors(a, variables);
            return a;
        }

        /// <summary>
        /// Begins a New Search corresponding with the <see cref="Solver"/> and
        /// <paramref name="agent"/>.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        protected abstract ISearchAgent NewSearch(ISearchAgent agent);

        /// <summary>
        /// Gets the <see cref="OptimizeVar"/> instances from the
        /// <see cref="ProblemSolverBase.ClrCreatedObjects"/>.
        /// </summary>
        protected IEnumerable<OptimizeVar> Optimizations => ClrCreatedObjects.OfType<OptimizeVar>().ToArray();

        /// <summary>
        /// Tries to Resolve the problem.
        /// </summary>
        /// <returns></returns>
        public override bool TryResolve()
        {
            using (var solver = new Solver(ModelName))
            {
                ReSeed(solver);

                PrepareVariables(solver);
                PrepareConstraints(solver);

                var e = EventArgs.Empty;

                /* It is important that search preparation include allocation of monitor(s) and
                 * DecisionBuilders. However, these cannot truly be known until the moment when
                 * the search is about ready to get under way. */

                /* Additionally, it is also very important to leave resolution of NewSearch to the
                 * derived class, since we cannot really know what sort of Phase DecisionBuilder
                 * will be required until that moment. */

                using (var sa = PrepareSearch(solver, Variables.ToArray()))
                {
                    OnResolving(e);

                    NewSearch(sa);
                }

                OnResolved(e);
            }
            return true;
        }

        // TODO: TBD: Model Export/Load features are not yet available in the current NuGet published package.
        // TODO: TBD: when they do become available, consider whether there is value exporting the model...

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // TODO: TBD: dispose of disposable objects here?
        }
    }
}
