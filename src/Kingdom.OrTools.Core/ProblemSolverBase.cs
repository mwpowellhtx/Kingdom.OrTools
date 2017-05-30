using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Establishes a loosely coupled problem solver for use throughout.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    public abstract class ProblemSolverBase<TSolver, TVariable> : IProblemSolver<TSolver>
        where TSolver : class
    {
        /// <summary>
        /// Gets the Solver.
        /// </summary>
        public virtual TSolver Solver { get; protected set; }

        /// <summary>
        /// Override to return the <typeparamref name="TVariable"/> instances corresponding to
        /// this Problem Solver. It is suggested to capture your Variables in the most-derived
        /// possible class necessary to support your modeling requirements.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        protected abstract IEnumerable<TVariable> GetVariables(TSolver solver);

        /// <summary>
        /// Gets the ModelName.
        /// </summary>
        protected string ModelName { get; private set; }

        /// <summary>
        /// Gets the ClrCreatedObjects.
        /// </summary>
        public IList<object> ClrCreatedObjects { get; } = new List<object>();

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        protected ProblemSolverBase(string modelName)
        {
            ModelName = modelName;
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
            if (IsDisposed || !disposing)
            {
                return;
            }
            this.DisposeHost();
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Establishes an <typeparamref name="TAspect"/> based
    /// <see cref="ProblemSolverBase{TSolver,TVariable}"/> class.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <typeparam name="TConstraint"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    public abstract class ProblemSolverBase<TSolver, TVariable, TConstraint, TAspect>
        : ProblemSolverBase<TSolver, TVariable>
            , IProblemSolver<TSolver, TVariable, TConstraint, TAspect>
        where TSolver : class
        where TVariable : class
        where TConstraint : class
        where TAspect : IProblemSolverAspect<TSolver, TVariable, TConstraint, TAspect>
    {
        /// <summary>
        /// Gets the Aspects involved. Aspects represents unique parts of the Problem Solver
        /// model, with variables that standalone for each part, and which may be intersected
        /// in order to discover new variables. And similarly for constraints.
        /// </summary>
        protected IEnumerable<TAspect> Aspects { get; }

        /// <summary>
        /// Returns the Variables corresponding to each of the <see cref="Aspects"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        protected override IEnumerable<TVariable> GetVariables(TSolver solver)
            => Aspects?.SelectMany(a => a.GetVariables(solver).Select(x => x.TrackClrObject(a)));

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="aspects"></param>
        protected ProblemSolverBase(string modelName, IEnumerable<TAspect> aspects)
            : base(modelName)
        {
            Aspects = (aspects ?? new TAspect[] {}).ToArray();
        }

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
            {
                return;
            }

            foreach (var a in Aspects)
            {
                a.DisposeHost();
                a.Dispose();
            }

            base.Dispose(true);
        }
    }
}
