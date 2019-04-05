using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using static MPSolverParameters.IncrementalityValues;

    // ReSharper disable once IdentifierTypo
    /// <summary>
    /// Provides a more C-Sharp friendly view of the <see cref="MPSolverParameters.IncrementalityValues"/> enumeration.
    /// </summary>
    public enum MpParamIncrementalityValues
    {
        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="INCREMENTALITY_OFF"/>
        /// </summary>
        IncrementalityOff = INCREMENTALITY_OFF,

        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="INCREMENTALITY_ON"/>
        /// </summary>
        IncrementalityOn = INCREMENTALITY_ON
    }
}
