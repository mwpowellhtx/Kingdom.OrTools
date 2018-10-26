using System;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Establishes a loosely coupled problem solver for use throughout.
    /// </summary>
    /// <inheritdoc cref="IClrObjectHost"/>
    public interface IProblemSolver : IClrObjectHost, IDisposable
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

    /// <summary>
    /// Intermediate interface for search agent purposes.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <inheritdoc />
    public interface IProblemSolver<out TSolver> : IProblemSolver
    {
        /// <summary>
        /// Gets the Solver corresponding to the Problem Solver.
        /// </summary>
        TSolver Solver { get; }
    }

    /// <summary>
    /// <typeparamref name="TAspect"/> based <see cref="IProblemSolver"/>.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <typeparam name="TConstraint"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    /// <inheritdoc />
    public interface IProblemSolver<out TSolver, TVariable, TConstraint, TAspect> : IProblemSolver<TSolver>
        where TSolver : class
        where TVariable : class
        where TConstraint : class
        where TAspect : IProblemSolverAspect<TSolver, TVariable, TConstraint, TAspect>
    {
    }
}
