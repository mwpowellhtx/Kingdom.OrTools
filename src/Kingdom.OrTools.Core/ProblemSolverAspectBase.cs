using System;
using System.Collections.Generic;

namespace Kingdom.OrTools
{
    /// <inheritdoc />
    public abstract class ProblemSolverAspectBase<TSolver, TAspect>
        : IProblemSolverAspect<TSolver, TAspect>
        where TSolver : class
        where TAspect : ProblemSolverAspectBase<TSolver, TAspect>
    {
        /// <inheritdoc />
        public IList<object> ClrCreatedObjects { get; } = new List<object>();

        /// <summary>
        /// Gets whether IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

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
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }

    /// <inheritdoc cref="IProblemSolverAspect{TSolver,TSource,TVariable,TConstraint,TAspect}"/>
    /// <see cref="IProblemSolverAspect{TSolver,TSource,TVariable,TConstraint,TAspect}"/>
    public abstract class ProblemSolverAspectBase<TSolver, TSource, TVariable, TConstraint, TAspect>
        : ProblemSolverAspectBase<TSolver, TAspect>
            , IProblemSolverAspect<TSolver, TSource, TVariable, TConstraint, TAspect>
        where TSolver : class
        where TSource : class
        where TVariable : class
        where TConstraint : class
        where TAspect : ProblemSolverAspectBase<TSolver, TSource, TVariable, TConstraint, TAspect>
    {
        /// <inheritdoc />
        public virtual IEnumerable<TVariable> GetVariables(TSource source)
        {
            yield break;
        }

        /// <inheritdoc />
        public abstract IEnumerable<TConstraint> GetConstraints(TSource source);

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Gets any <typeparamref name="TVariable"/> instances corresponding with this aspect.
        /// </summary>
        protected virtual IList<TVariable> IntersectedVariables { get; } = new List<TVariable>();

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Get any <typeparamref name="TConstraint"/> instances corresponding with this aspect.
        /// </summary>
        protected virtual IList<TConstraint> IntersectedConstraints { get; } = new List<TConstraint>();

        /// <inheritdoc />
        public virtual IEnumerable<TVariable> IntersectVariables(TSource source, TAspect otherAspect)
        {
            yield break;
        }

        /// <inheritdoc />
        public virtual IEnumerable<TConstraint> Intersect(TSource source, TAspect otherAspect)
        {
            yield break;
        }
    }
}
