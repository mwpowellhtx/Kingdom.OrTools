using System;
using System.Collections.Generic;

namespace Kingdom.OrTools
{
    /// <summary>
    /// Represents a <see cref="IProblemSolver{TSolver}"/> aspect.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    /// <inheritdoc cref="IClrObjectHost"/>
    public interface IProblemSolverAspect<TSolver, TAspect> : IClrObjectHost, IDisposable
        where TSolver : class
        where TAspect : IProblemSolverAspect<TSolver, TAspect>
    {
    }

    // TODO: TBD: it could be that "solver" is the "source" under some circumstances, i.e. Constraint Solver
    // TODO: TBD: whereas the "source" can be the "model", i.e. SAT-CP ...
    /// <summary>
    /// Represents a <see cref="IProblemSolver{TSolver}"/> aspect.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TSource">Source from which to draw the <typeparamref name="TVariable"/> set.</typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <typeparam name="TConstraint"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    /// <inheritdoc />
    public interface IProblemSolverAspect<TSolver, in TSource, out TVariable, out TConstraint, TAspect>
        : IProblemSolverAspect<TSolver, TAspect>
        where TSolver : class
        where TSource : class
        where TVariable : class
        where TConstraint : class
        where TAspect : IProblemSolverAspect<TSolver, TSource, TVariable, TConstraint, TAspect>
    {
        /// <summary>
        /// Returns the Variables corresponding with this Aspect.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>This interface does need to maintain a <typeparamref name="TVariable"/>
        /// getting contract.</remarks>
        IEnumerable<TVariable> GetVariables(TSource source);

        /// <summary>
        /// Returns the Constraints associated with this aspect.
        /// </summary>
        /// <param name="source"></param>
        IEnumerable<TConstraint> GetConstraints(TSource source);

        /// <summary>
        /// Returns any Variables discovered by Intersecting this Aspect with the
        /// <paramref name="otherAspect"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="otherAspect"></param>
        /// <returns></returns>
        IEnumerable<TVariable> IntersectVariables(TSource source, TAspect otherAspect);

        /// <summary>
        /// Returns any Constraints discovered by Intersecting this Aspect with the
        /// <paramref name="otherAspect"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="otherAspect"></param>
        /// <returns></returns>
        IEnumerable<TConstraint> Intersect(TSource source, TAspect otherAspect);
    }
}
