namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provides a <see cref="Solver"/> based host interface.
    /// </summary>
    public interface IOrClrObjectHost : IClrObjectHost
    {
        /// <summary>
        /// Gets the Solver.
        /// </summary>
        Solver Solver { get; }
    }
}
