using System;
using System.Collections.Generic;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Establishes a loosely coupled problem solver for use throughout.
    /// </summary>
    public abstract class ProblemSolverBase : IProblemSolver, IClrObjectHost
    {
        /// <summary>
        /// Gets the ModelName.
        /// </summary>
        protected string ModelName { get; private set; }

        /// <summary>
        /// ClrCreatedObjects backing field.
        /// </summary>
        private readonly Lazy<IList<object>> _lazyClrCreatedObjects;

        /// <summary>
        /// Gets the ClrCreatedObjects.
        /// </summary>
        public IList<object> ClrCreatedObjects => _lazyClrCreatedObjects.Value;

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        protected ProblemSolverBase(string modelName)
        {
            ModelName = modelName;
            _lazyClrCreatedObjects = new Lazy<IList<object>>(() => new List<object>());
        }

        /// <summary>
        /// Resolves the problem.
        /// </summary>
        public void Resolve()
        {
            TryResolve();
        }

        /// <summary>
        /// Tries to <see cref="Resolve"/> the problem.
        /// </summary>
        /// <returns></returns>
        public abstract bool TryResolve();

        /// <summary>
        /// Occurs just after Preparation and prior to solver initiating a new Search.
        /// </summary>
        public virtual event EventHandler<EventArgs> Resolving;

        /// <summary>
        /// Occurs On <see cref="Resolving"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnResolving(EventArgs e)
        {
            Resolving?.Invoke(this, e);
        }

        /// <summary>
        /// Occurs when the Solver has completely Resolved.
        /// </summary>
        public virtual event EventHandler<EventArgs> Resolved;

        /// <summary>
        /// Occurs On <see cref="Resolved"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnResolved(EventArgs e)
        {
            Resolved?.Invoke(this, e);
        }

        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        public virtual bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;
            ClrCreatedObjects.Clear();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            Dispose(true);
            IsDisposed = true;
        }
    }
}
