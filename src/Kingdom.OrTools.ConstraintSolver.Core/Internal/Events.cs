using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Breakable interface used to signal when a control decision should break from a loop.
    /// </summary>
    public interface IBreakable
    {
        /// <summary>
        /// Gets or sets whether ShouldBreak.
        /// </summary>
        bool ShouldBreak { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class AgentEventArgsBase : EventArgs, IBreakable
    {
        /// <summary>
        /// Gets the Solver.
        /// </summary>
        public Solver Solver { get; }

        /// <summary>
        /// Gets or sets whether Should Break.
        /// </summary>
        public bool ShouldBreak { get; set; }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="solver"></param>
        protected AgentEventArgsBase(Solver solver)
        {
            Solver = solver;
        }
    }

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
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
