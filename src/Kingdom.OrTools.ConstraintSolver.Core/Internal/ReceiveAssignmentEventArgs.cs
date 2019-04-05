namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <inheritdoc />
    public class ReceiveAssignmentEventArgs : AgentEventArgsBase
    {
        /// <summary>
        /// Gets the Assignment.
        /// </summary>
        public Assignment Assignment { get; internal set; }

        internal ReceiveAssignmentEventArgs(Solver solver)
            : base(solver)
        {
        }

        internal ReceiveAssignmentEventArgs(Solver solver, Assignment assignment)
            : base(solver)
        {
            Assignment = assignment;
        }
    }
}
