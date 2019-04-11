namespace Kingdom.OrTools.Sat
{
    using SolverStatus = Google.OrTools.Sat.CpSolverStatus;

    /// <summary>
    /// Adapts the <see cref="SolverStatus"/> into the adapter.
    /// </summary>
    public enum CpSolverStatus
    {
        /// <summary>
        /// <see cref="SolverStatus.Unknown"/>
        /// </summary>
        Unknown = SolverStatus.Unknown,

        /// <summary>
        /// <see cref="SolverStatus.ModelInvalid"/>
        /// </summary>
        ModelInvalid = SolverStatus.ModelInvalid,

        /// <summary>
        /// <see cref="SolverStatus.Feasible"/>
        /// </summary>
        Feasible = SolverStatus.Feasible,

        /// <summary>
        /// <see cref="SolverStatus.Infeasible"/>
        /// </summary>
        Infeasible = SolverStatus.Infeasible,

        /// <summary>
        /// <see cref="SolverStatus.Optimal"/>
        /// </summary>
        Optimal = SolverStatus.Optimal
    }
}
