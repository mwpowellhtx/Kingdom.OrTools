namespace Kingdom.OrTools.ConstraintSolver
{
    /// <summary>
    /// Breakable interface used to signal when a control decision should break from a loop.
    /// </summary>
    public interface IBreakable
    {
        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Gets or sets whether ShouldBreak.
        /// </summary>
        bool ShouldBreak { get; set; }
    }
}
