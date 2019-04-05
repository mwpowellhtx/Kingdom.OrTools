namespace Kingdom.OrTools.LinearSolver
{
    using BasisStatus = Google.OrTools.LinearSolver.Solver.BasisStatus;
    using static Google.OrTools.LinearSolver.Solver.BasisStatus;

    /// <summary>
    /// Provides a C-Sharp friendly view of the <see cref="BasisStatus"/>.
    /// </summary>
    public enum LinearBasisStatus
    {
        /// <summary>
        /// <see cref="FREE"/>
        /// </summary>
        Free = FREE,

        /// <summary>
        /// <see cref="AT_LOWER_BOUND"/>
        /// </summary>
        AtLowerBound = AT_LOWER_BOUND,

        /// <summary>
        /// <see cref="AT_UPPER_BOUND"/>
        /// </summary>
        AtUpperBound = AT_UPPER_BOUND,

        /// <summary>
        /// <see cref="FIXED_VALUE"/>
        /// </summary>
        FixedValue = FIXED_VALUE,

        /// <summary>
        /// <see cref="BASIC"/>
        /// </summary>
        Basic = BASIC
    }
}
