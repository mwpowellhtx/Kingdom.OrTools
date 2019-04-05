namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <inheritdoc />
    public class SearchAgentPredicateEventArgs : AgentEventArgsBase
    {
        /// <summary>
        /// Gets the Collector.
        /// </summary>
        public SolutionCollector Collector { get; }

        internal SearchAgentPredicateEventArgs(Solver solver, SolutionCollector collector)
            : base(solver)
        {
            Collector = collector;
        }
    }
}
