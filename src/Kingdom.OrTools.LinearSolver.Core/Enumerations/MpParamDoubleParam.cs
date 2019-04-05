using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using static MPSolverParameters.DoubleParam;

    /// <summary>
    /// Provides a more C-Sharp friendly view of the <see cref="MPSolverParameters.DoubleParam"/> enumeration.
    /// </summary>
    public enum MpParamDoubleParam
    {
        /// <summary>
        /// <see cref="DUAL_TOLERANCE"/>
        /// </summary>
        DualTolerance = DUAL_TOLERANCE,

        /// <summary>
        /// <see cref="PRIMAL_TOLERANCE"/>
        /// </summary>
        PrimalTolerance = PRIMAL_TOLERANCE,

        /// <summary>
        /// <see cref="RELATIVE_MIP_GAP"/>
        /// </summary>
        RelativeMipGap = RELATIVE_MIP_GAP
    }
}
