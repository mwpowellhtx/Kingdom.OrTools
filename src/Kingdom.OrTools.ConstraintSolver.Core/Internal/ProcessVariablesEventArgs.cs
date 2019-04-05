using System.Collections.Generic;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <inheritdoc />
    public class ProcessVariablesEventArgs : AgentEventArgsBase
    {
        /// <summary>
        /// Gets the Variables.
        /// </summary>
        public IEnumerable<IntVar> Variables { get; }

        internal ProcessVariablesEventArgs(Solver solver, params IntVar[] variables)
            : base(solver)
        {
            Variables = variables;
        }
    }
}
