using System;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Search agent for purposes of facilitating search through the configured
    /// <see cref="Solver"/>. The main purpose of the Search Agent is to manage the Begin and
    /// End Search boundaries. Only what needs to be exposed in order to facilitate the Search
    /// is exposed, including Predication, and Assignment and Variable processing.
    /// </summary>
    public interface ISearchAgent : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="Google.OrTools.ConstraintSolver.Solver"/> corresponding with the
        /// <see cref="ISearchAgent"/>.
        /// </summary>
        Solver Solver { get; }

        /// <summary>
        /// Gets the Variables.
        /// </summary>
        IntVar[] Variables { get; }

        /// <summary>
        /// Prepares the <see cref="SearchMonitor"/> given the <paramref name="createMonitor"/>.
        /// Additionally, as it turns out, Optimizations such as Minimize or Maximize are
        /// themselves a kind of <see cref="OptimizeVar"/>, which derives from
        /// <see cref="SearchMonitor"/>. In other words, optimizations are introduced to the
        /// underlying <see cref="Solver"/> model via the Monitors.
        /// </summary>
        /// <param name="createMonitor"></param>
        /// <returns></returns>
        ISearchAgent Monitor(Func<ISearchAgent, SearchMonitor> createMonitor);

        /// <summary>
        /// Returns whether the Search Agent Has Monitor of type <see cref="SearchMonitor"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasMonitor<T>() where T : SearchMonitor;

        /// <summary>
        /// Returns whether the Search Agent Has Monitor of type <see cref="SolutionCollector"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasSolutionCollector<T>() where T : SolutionCollector;

        /// <summary>
        /// Initiates a NewSearch utilizing the <paramref name="factory"/> provided.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        ISearchAgent NewSearch(Func<ISearchAgent, DecisionBuilder> factory);

        /// <summary>
        /// Predicated event.
        /// </summary>
        event EventHandler<SearchAgentPredicateEventArgs> Predicated;

        /// <summary>
        /// NextAssignment event.
        /// </summary>
        event EventHandler<ReceiveAssignmentEventArgs> NextAssignment;

        /// <summary>
        /// ProcessVariables event.
        /// </summary>
        event EventHandler<ProcessVariablesEventArgs> ProcessVariables;
    }
}
