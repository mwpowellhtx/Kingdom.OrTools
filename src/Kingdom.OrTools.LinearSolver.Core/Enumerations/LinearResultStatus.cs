using System;

namespace Kingdom.OrTools.LinearSolver
{
    using ResultStatus = Google.OrTools.LinearSolver.Solver.ResultStatus;
    using static Google.OrTools.LinearSolver.Solver.ResultStatus;

    /// <summary>
    /// Provides a C-Sharp friendly view of the <see cref="ResultStatus"/> enumeration.
    /// </summary>
    public enum LinearResultStatus
    {
        /// <summary>
        /// <see cref="OPTIMAL"/>
        /// </summary>
        Optimal = OPTIMAL,

        /// <summary>
        /// <see cref="FEASIBLE"/>
        /// </summary>
        Feasible = FEASIBLE,

        /// <summary>
        /// <see cref="INFEASIBLE"/>
        /// </summary>
        Infeasible = INFEASIBLE,

        /// <summary>
        /// <see cref="UNBOUNDED"/>
        /// </summary>
        Unbounded = UNBOUNDED,

        /// <summary>
        /// <see cref="ABNORMAL"/>
        /// </summary>
        Abnormal = ABNORMAL,

        /// <summary>
        /// No longer used.
        /// </summary>
        [Obsolete("This one is no longer used.")]
        ModelInvalid = 5,

        /// <summary>
        /// <see cref="NOT_SOLVED"/>
        /// </summary>
        NotSolved = NOT_SOLVED
    }
}
