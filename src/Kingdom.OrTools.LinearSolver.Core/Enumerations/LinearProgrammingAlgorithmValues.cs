using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using static MPSolverParameters.LpAlgorithmValues;

    /// <summary>
    /// Provides a more C-Sharp friendly view of the <see cref="MPSolverParameters.LpAlgorithmValues"/> enumeration.
    /// </summary>
    public enum LinearProgrammingAlgorithmValues
    {
        /// <summary>
        /// <see cref="DUAL"/>
        /// </summary>
        Dual = DUAL,

        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="PRIMAL"/>
        /// </summary>
        Primal = PRIMAL,

        /// <summary>
        /// <see cref="BARRIER"/>
        /// </summary>
        Barrier = BARRIER
    }
}
