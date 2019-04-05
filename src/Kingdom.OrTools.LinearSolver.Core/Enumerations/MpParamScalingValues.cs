using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using static MPSolverParameters.ScalingValues;

    /// <summary>
    /// Provides a more C-Sharp friendly view of the <see cref="MPSolverParameters.ScalingValues"/> enumeration.
    /// </summary>
    public enum MpParamScalingValues
    {
        /// <summary>
        /// <see cref="SCALING_OFF"/>
        /// </summary>
        ScalingOff = SCALING_OFF,

        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// <see cref="SCALING_ON"/>
        /// </summary>
        ScalingOn = SCALING_ON
    }
}
