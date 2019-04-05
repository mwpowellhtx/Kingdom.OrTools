using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using static MPSolverParameters.PresolveValues;

    // ReSharper disable once IdentifierTypo
    /// <summary>
    /// Provides a more C-Sharp friendly view of the <see cref="MPSolverParameters.PresolveValues"/> enumeration.
    /// </summary>
    public enum MpParamPresolveValues
    {
        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="PRESOLVE_OFF"/>
        /// </summary>
        PresolveOff = PRESOLVE_OFF,

        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="PRESOLVE_ON"/>
        /// </summary>
        PresolveOn = PRESOLVE_ON
    }
}
