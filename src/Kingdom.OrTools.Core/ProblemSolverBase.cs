using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Establishes a loosely coupled Problem Solver for use throughout.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <inheritdoc />
    public abstract class ProblemSolverBase<TSolver, TVariable> : IProblemSolver<TSolver>
        where TSolver : class
    {
        /// <inheritdoc />
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
        protected string ModelName { get; }

        /// <inheritdoc />
        public IList<object> ClrCreatedObjects { get; } = new List<object>();

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        protected ProblemSolverBase(string modelName)
        {
            ModelName = modelName;
        }

        /// <inheritdoc />
        public void Resolve()
        {
            TryResolve();
        }

        /// <inheritdoc />
        public abstract bool TryResolve();

        /// <inheritdoc />
        public virtual event EventHandler<EventArgs> Resolving;

        /// <summary>
        /// Occurs On <see cref="Resolving"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnResolving(EventArgs e)
        {
            Resolving?.Invoke(this, e);
        }

        /// <inheritdoc />
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

            foreach (var obj in ClrCreatedObjects)
            {
                if (obj != null && obj is IDisposable disposableObj)
                {
                    disposableObj.Dispose();
                }
            }

            ClrCreatedObjects.Clear();
        }

        /// <inheritdoc />
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
    /// <inheritdoc cref="ProblemSolverBase{TSolver,TVariable}"/>
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

        /// <inheritdoc />
        protected override IEnumerable<TVariable> GetVariables(TSolver solver)
            => Aspects?.SelectMany(a => a.GetVariables(solver).Select(x => x.TrackClrObject(a)));

        /// <inheritdoc />
        protected ProblemSolverBase(string modelName, IEnumerable<TAspect> aspects)
            : base(modelName)
        {
            Aspects = (aspects ?? new TAspect[] {}).ToArray();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
            {
                return;
            }

            foreach (var a in Aspects.OfType<IDisposable>())
            {
                a.Dispose();
            }

            base.Dispose(true);
        }
    }
}
