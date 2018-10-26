using System;
using System.Collections.Generic;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Represents a Problem Solver Aspect base class.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
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

    /// <summary>
    /// Represents Problem Solver Aspect base class with special consideration for
    /// <typeparamref name="TVariable"/> and <typeparamref name="TConstraint"/> concerns.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <typeparam name="TConstraint"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    /// <inheritdoc cref="ProblemSolverAspectBase{TSolver,TAspect}"/>
    public abstract class ProblemSolverAspectBase<TSolver, TVariable, TConstraint, TAspect>
        : ProblemSolverAspectBase<TSolver, TAspect>
            , IProblemSolverAspect<TSolver, TVariable, TConstraint, TAspect>
        where TSolver : class
        where TVariable : class
        where TConstraint : class
        where TAspect : ProblemSolverAspectBase<TSolver, TVariable, TConstraint, TAspect>
    {
        /// <inheritdoc />
        public abstract IEnumerable<TVariable> GetVariables(TSolver solver);

        /// <inheritdoc />
        public abstract IEnumerable<TConstraint> GetConstraints(TSolver solver);

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
        public virtual IEnumerable<TVariable> IntersectVariables(TSolver solver, TAspect otherAspect)
        {
            yield break;
        }

        /// <inheritdoc />
        public virtual IEnumerable<TConstraint> Intersect(TSolver solver, TAspect otherAspect)
        {
            yield break;
        }
    }
}
