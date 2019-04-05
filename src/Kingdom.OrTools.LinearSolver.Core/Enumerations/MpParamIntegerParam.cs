using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using static MPSolverParameters.IntegerParam;

    /// <summary>
    /// Provides a more C-Sharp friendly view of the <see cref="MPSolverParameters.IntegerParam"/> enumeration.
    /// </summary>
    public enum MpParamIntegerParam
    {
        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="PRESOLVE"/>
        /// </summary>
        Presolve = PRESOLVE,

        /// <summary>
        /// <see cref="LP_ALGORITHM"/>
        /// </summary>
        LinearProgrammingAlgorithm = LP_ALGORITHM,

        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="INCREMENTALITY"/>
        /// </summary>
        Incrementality = INCREMENTALITY,

        /// <summary>
        /// <see cref="SCALING"/>
        /// </summary>
        Scaling = SCALING
    }
}
