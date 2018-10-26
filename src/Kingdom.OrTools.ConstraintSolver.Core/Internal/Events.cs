using System;
using System.Collections.Generic;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Breakable interface used to signal when a control decision should break from a loop.
    /// </summary>
    public interface IBreakable
    {
        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Gets or sets whether ShouldBreak.
        /// </summary>
        bool ShouldBreak { get; set; }
    }

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
