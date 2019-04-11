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
        /// Occurs when the Solver has completely Resolved. Note that Resolved does
        /// not necessarily mean Solved. Only that the Problem Solver has completed
        /// the resolution procedure.
        /// </summary>
        event EventHandler<EventArgs> Resolved;
    }

    /// <summary>
    /// Intermediate interface for search agent purposes.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <inheritdoc />
    public interface IProblemSolver<out TSolver, out TSource> : IProblemSolver
        where TSolver : class
        where TSource : class
    {
        /// <summary>
        /// Gets the Solver corresponding to the Problem Solver.
        /// </summary>
        TSolver Solver { get; }

        /// <summary>
        /// Gets the Source corresponding to the Problem Solver.
        /// </summary>
        TSource Source { get; }
    }

    /// <summary>
    /// <typeparamref name="TAspect"/> based <see cref="IProblemSolver"/>.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <typeparam name="TConstraint"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    /// <inheritdoc />
    public interface IProblemSolver<out TSolver, out TSource, TVariable, TConstraint, TAspect> : IProblemSolver<TSolver, TSource>
        where TSolver : class
        where TSource : class
        where TVariable : class
        where TConstraint : class
        where TAspect : IProblemSolverAspect<TSolver, TSource, TVariable, TConstraint, TAspect>
    {
    }
}
