using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;
    using static IntVarStrategy;
    using static IntValueStrategy;

    // TODO: TBD: consider a fluent configuration-based approach...

    /// <summary>
    /// or-tools-based Constraint Programming problem solver.
    /// </summary>
    /// <see cref="!:http://code.google.com/p/or-tools/"/>
    public abstract class OrProblemSolverBase : ProblemSolverBase, IOrProblemSolver
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
        protected virtual int GetSolverSeed()
        {
            return new Random().Next();
        }

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
        /// <returns></returns>
        protected abstract IEnumerable<IntVar> PrepareVariables(Solver solver);

        /// <summary>
        /// Prepares the solver constraints. No need to add the Constraints to the
        /// <paramref name="solver"/>. Just prepare and return them. The parent class will handle
        /// the responsibility of adding them to the <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        protected abstract IEnumerable<Constraint> PrepareConstraints(Solver solver);

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
        /// Solved event.
        /// </summary>
        public event EventHandler<EventArgs> Solved;

        /// <summary>
        /// Raises the <see cref="Solved"/> event.
        /// </summary>
        /// <param name="e"></param>
        private void OnSolved(EventArgs e)
        {
            Solved?.Invoke(this, e);
        }

        /// <summary>
        /// <see cref="ISearchAgent"/> ProcessVariables event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnProcessVariables(object sender, ProcessVariablesEventArgs e)
        {
            OnSolved(EventArgs.Empty);
        }

        /// <summary>
        /// Begins a New Search corresponding with the <see cref="Solver"/> and
        /// <paramref name="agent"/>.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        protected virtual ISearchAgent NewSearch(ISearchAgent agent)
        {
            agent.ProcessVariables -= OnProcessVariables;
            agent.ProcessVariables += OnProcessVariables;

            return agent.NewSearch(
                a => a.Solver.MakePhase(a.Variables, IntVarSimple, IntValueSimple)
                    .TrackClrObject(this)
            );
        }

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
                // Capture the Solver instance for local usage.
                var s = solver;

                ReSeed(solver);

                /* Capture the Variables and Constraints. At the same time, Track them as CLR objects,
                 * and Add the Constraints to the Solver. */

                var variables = PrepareVariables(s).Select(x => x.TrackClrObject(this)).ToArray();

                foreach (var c in PrepareConstraints(s).ToArray())
                {
                    s.Add(c.TrackClrObject(this));
                }

                var e = EventArgs.Empty;

                /* It is important that search preparation include allocation of monitor(s) and
                 * DecisionBuilders. However, these cannot truly be known until the moment when
                 * the search is about ready to get under way. */

                /* Additionally, it is also very important to leave resolution of NewSearch to the
                 * derived class, since we cannot really know what sort of Phase DecisionBuilder
                 * will be required until that moment. */

                using (var sa = PrepareSearch(s, variables))
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
