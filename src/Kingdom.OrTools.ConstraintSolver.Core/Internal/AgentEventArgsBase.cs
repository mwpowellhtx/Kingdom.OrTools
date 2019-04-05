using System;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// 
    /// </summary>
    /// <inheritdoc cref="EventArgs"/>
    public abstract class AgentEventArgsBase : EventArgs, IBreakable
    {
        /// <summary>
        /// Gets the Solver.
        /// </summary>
        public Solver Solver { get; }

        /// <inheritdoc />
        public bool ShouldBreak { get; set; }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="solver"></param>
        /// <inheritdoc />
        protected AgentEventArgsBase(Solver solver)
        {
            Solver = solver;
        }
    }
}
