using System.Collections.Generic;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Represents a Problem Solver Aspect base class.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    public abstract class ProblemSolverAspectBase<TSolver, TAspect>
        : IProblemSolverAspect<TSolver, TAspect>
        where TSolver : class
        where TAspect : ProblemSolverAspectBase<TSolver, TAspect>
    {
        /// <summary>
        /// Gets the ClrCreatedObjects.
        /// </summary>
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
                // ReSharper disable once RedundantJumpStatement
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
    /// Represents Problem Solver Aspect base class with special consideration for
    /// <typeparamref name="TVariable"/> and <typeparamref name="TConstraint"/> concerns.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <typeparam name="TConstraint"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    public abstract class ProblemSolverAspectBase<TSolver, TVariable, TConstraint, TAspect>
        : ProblemSolverAspectBase<TSolver, TAspect>
            , IProblemSolverAspect<TSolver, TVariable, TConstraint, TAspect>
        where TSolver : class
        where TVariable : class
        where TConstraint : class
        where TAspect : ProblemSolverAspectBase<TSolver, TVariable, TConstraint, TAspect>
    {
        /// <summary>
        /// Returns the <typeparamref name="TVariable"/> instances related to this aspect.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public abstract IEnumerable<TVariable> GetVariables(TSolver solver);

        /// <summary>
        /// Returns the <typeparamref name="TConstraint"/> instances related to this aspect.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public abstract IEnumerable<TConstraint> GetConstraints(TSolver solver);

        /// <summary>
        /// Gets any <typeparamref name="TVariable"/> instances corresponding with this aspect.
        /// </summary>
        protected virtual IList<TVariable> IntersectedVariables { get; } = new List<TVariable>();

        /// <summary>
        /// Get any <typeparamref name="TConstraint"/> instances corresponding with this aspect.
        /// </summary>
        protected virtual IList<TConstraint> IntersectedConstraints { get; } = new List<TConstraint>();

        /// <summary>
        /// Returns any <typeparamref name="TVariable"/> instances discovered by Intersecting this
        /// aspect with the <paramref name="otherAspect"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="otherAspect"></param>
        /// <returns></returns>
        public virtual IEnumerable<TVariable> IntersectVariables(TSolver solver, TAspect otherAspect)
        {
            yield break;
        }

        /// <summary>
        /// Returns any <typeparamref name="TConstraint"/> instances discovered by Intersecting
        /// this aspect with the <paramref name="otherAspect"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="otherAspect"></param>
        /// <returns></returns>
        public virtual IEnumerable<TConstraint> Intersect(TSolver solver, TAspect otherAspect)
        {
            yield break;
        }
    }
}
