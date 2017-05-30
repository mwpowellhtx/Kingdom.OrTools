using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Search agent for purposes of facilitating search through the configured <see cref="Solver"/>.
    /// </summary>
    internal class SearchAgent : ISearchAgent
    {
        private readonly IProblemSolver<Solver> _problemSolver;

        /// <summary>
        /// Gets the Host.
        /// </summary>
        /// <see cref="_problemSolver"/>
        private IClrObjectHost Host => _problemSolver;

        /// <summary>
        /// Gets the Solver.
        /// </summary>
        /// <see cref="IProblemSolver{Solver}"/>
        public Solver Solver => _problemSolver?.Solver;

        /// <summary>
        /// Predicated event.
        /// </summary>
        public event EventHandler<SearchAgentPredicateEventArgs> Predicated;

        private SearchAgentPredicateEventArgs OnPredicated()
        {
            var e = new SearchAgentPredicateEventArgs(Solver, Collector);
            Predicated?.Invoke(this, e);
            return e;
        }

        public event EventHandler<ReceiveAssignmentEventArgs> NextAssignment;

        private ReceiveAssignmentEventArgs OnNextAssignment(ReadOnlyAssignmentCollection collection)
        {
            // Only engage the collection and potentially invoke the event when we have something to invoke.
            if (NextAssignment == null || !(collection.HasCollector && collection.Any()))
            {
                return new ReceiveAssignmentEventArgs(Solver);
            }
            var e = new ReceiveAssignmentEventArgs(Solver, collection[collection.Count - 1]);
            NextAssignment(this, e);
            return e;
        }

        /// <summary>
        /// Gets the Variables.
        /// </summary>
        public IntVar[] Variables { get; internal set; }

        public event EventHandler<ProcessVariablesEventArgs> ProcessVariables;

        private ProcessVariablesEventArgs OnProcessVariables(params IntVar[] variables)
        {
            var e = new ProcessVariablesEventArgs(Solver, variables);
            ProcessVariables?.Invoke(this, e);
            return e;
        }

        /// <summary>
        /// Gets or sets the Collector.
        /// </summary>
        private SolutionCollector Collector { get; set; }

        /// <summary>
        /// Gets the Monitors.
        /// </summary>
        private IList<SearchMonitor> Monitors { get; }

        /// <summary>
        /// Gets or sets the DecisionBuilder.
        /// </summary>
        private DecisionBuilder DecisionBuilder { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="problemSolver"></param>
        /// <param name="variables"></param>
        public SearchAgent(IProblemSolver<Solver> problemSolver, params IntVar[] variables)
        {
            _problemSolver = problemSolver;

            Variables = variables;

            // Prepare to receive some Monitors.
            Monitors = new List<SearchMonitor>();
        }

        /// <summary>
        /// Prepares a <see cref="SearchMonitor"/> given the <paramref name="createMonitor"/>. If
        /// the Monitor is a <see cref="SolutionCollector"/> then this is used as the
        /// <see cref="Collector"/>. Additionally, as it turns out, Optimizations such as Minimize
        /// or Maximize are themselves a kind of <see cref="OptimizeVar"/>, which derives from
        /// <see cref="SearchMonitor"/>. In other words, optimizations are introduced to the
        /// underlying <see cref="Solver"/> model via the Monitors.
        /// </summary>
        /// <param name="createMonitor"></param>
        /// <returns></returns>
        public ISearchAgent Monitor(Func<ISearchAgent, SearchMonitor> createMonitor)
        {
            var m = createMonitor(this).TrackClrObject(Host);

            var sc = m as SolutionCollector;

            if (sc != null)
            {
                Collector = sc;
            }

            Monitors.Add(m);

            return this;
        }

        /// <summary>
        /// Returns whether the Search Agent Has Monitor of type <see cref="SearchMonitor"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasMonitor<T>() where T : SearchMonitor => Monitors.OfType<T>().Any();

        /// <summary>
        /// Returns whether the Search Agent Has Monitor of type <see cref="SolutionCollector"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasSolutionCollector<T>() where T : SolutionCollector => HasMonitor<T>();

        /// <summary>
        /// Prepares a <see cref="DecisionBuilder"/> given the <paramref name="factory"/>.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public ISearchAgent NewSearch(Func<ISearchAgent, DecisionBuilder> factory)
        {
            // ReSharper disable once InvertIf
            if (Solver != null)
            {
                var db = DecisionBuilder = factory(this).TrackClrObject(Host);

                Solver.NewSearch(db, Monitors.ToArray());
            }

            return this;
        }

        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs the Search given the <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        private Solver PerformSearch(Solver solver)
        {
            // TODO: TBD: depending on the kind of collector, first, last, all, etc, may want to rethink this approach a bit...
            // TODO: TBD: particularly in how/when we process the assignments/variables...
            var collection = new ReadOnlyAssignmentCollection(Collector);

            // Evaluate Variables as an Array once.
            var variables = Variables.ToArray();

            // The Solver should be set but let's double check that just in case.
            while (solver?.NextSolution() == true)
            {
                // TODO: TBD: the only thing I don't like about this is the overhead involved maintaining events/handlers/args, etc
                if (OnPredicated().ShouldBreak) break;

                if (OnNextAssignment(collection).ShouldBreak) break;

                if (OnProcessVariables(variables).ShouldBreak) break;
            }

            return solver;
        }

        /// <summary>
        /// Tears down the Search afterwards.
        /// </summary>
        private void TearDown()
        {
            foreach (var m in Monitors)
            {
                m?.Dispose();
            }

            DecisionBuilder?.Dispose();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || IsDisposed)
            {
                return;
            }

            PerformSearch(Solver).EndSearch();

            TearDown();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }
}
