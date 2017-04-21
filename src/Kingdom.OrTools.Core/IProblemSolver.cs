using System;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Establishes a loosely coupled problem solver for use throughout.
    /// </summary>
    public interface IProblemSolver : IDisposable
    {
        /// <summary>
        /// Resolves the problem.
        /// </summary>
        void Resolve();

        /// <summary>
        /// Tries to <see cref="Resolve"/> the problem.
        /// </summary>
        /// <returns></returns>
        /// <see cref="Resolve"/>
        bool TryResolve();

        /// <summary>
        /// Occurs just after Preparation and prior to solver initiating a new Search.
        /// </summary>
        event EventHandler<EventArgs> Resolving;

        /// <summary>
        /// Occurs when the Solver has completely Resolved.
        /// </summary>
        event EventHandler<EventArgs> Resolved;
    }
}
