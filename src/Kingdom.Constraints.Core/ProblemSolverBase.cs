using System.Collections.Generic;

namespace Kingdom.Constraints
{
    /// <summary>
    /// Establishes a loosely coupled problem solver for use throughout.
    /// </summary>
    public abstract class ProblemSolverBase : IProblemSolver
    {
        /// <summary>
        /// Gets the ModelName.
        /// </summary>
        protected string ModelName { get; private set; }

        /// <summary>
        /// ClrCreatedObjects backing field.
        /// </summary>
        private readonly IList<object> _clrCreatedObjects;

        /// <summary>
        /// Gets the ClrCreatedObjects.
        /// </summary>
        protected IList<object> ClrCreatedObjects
        {
            get { return _clrCreatedObjects; }
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        protected ProblemSolverBase(string modelName)
        {
            ModelName = modelName;
            _clrCreatedObjects = new List<object>();
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

        #region Disposable Members

        /// <summary>
        /// Whether the object has been Disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        public virtual bool IsDisposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing) return;
            _clrCreatedObjects.Clear();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            Dispose(true);
            _disposed = true;
        }

        #endregion
    }
}
