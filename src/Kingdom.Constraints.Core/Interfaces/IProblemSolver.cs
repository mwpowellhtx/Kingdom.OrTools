using System;

namespace Kingdom.Constraints
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
    }
}
