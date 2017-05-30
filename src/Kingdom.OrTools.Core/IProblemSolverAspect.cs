using System;
using System.Collections.Generic;

namespace Kingdom.OrTools
{
    // ReSharper disable once UnusedTypeParameter
    /// <summary>
    /// Represents a <see cref="IProblemSolver{TSolver}"/> aspect.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    public interface IProblemSolverAspect<TSolver, TAspect> : IClrObjectHost, IDisposable
        where TSolver : class
        where TAspect : IProblemSolverAspect<TSolver, TAspect>
    {
    }

    /// <summary>
    /// Represents a <see cref="IProblemSolver{TSolver}"/> aspect.
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    /// <typeparam name="TVariable"></typeparam>
    /// <typeparam name="TConstraint"></typeparam>
    /// <typeparam name="TAspect"></typeparam>
    public interface IProblemSolverAspect<TSolver, out TVariable, out TConstraint, TAspect>
        : IProblemSolverAspect<TSolver, TAspect>
        where TSolver : class
        where TVariable : class
        where TConstraint : class
        where TAspect : IProblemSolverAspect<TSolver, TVariable, TConstraint, TAspect>
    {
        /// <summary>
        /// Returns the Variables corresponding with this aspect.
        /// </summary>
        /// <param name="solver"></param>
        IEnumerable<TVariable> GetVariables(TSolver solver);

        /// <summary>
        /// Returns the Constraints associated with this aspect.
        /// </summary>
        /// <param name="solver"></param>
        IEnumerable<TConstraint> GetConstraints(TSolver solver);

        /// <summary>
        /// Returns any Variables discovered by Intersecting this Aspect with the
        /// <paramref name="otherAspect"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="otherAspect"></param>
        /// <returns></returns>
        IEnumerable<TVariable> IntersectVariables(TSolver solver, TAspect otherAspect);

        /// <summary>
        /// Returns any Constraints discovered by Intersecting this Aspect with the
        /// <paramref name="otherAspect"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="otherAspect"></param>
        /// <returns></returns>
        IEnumerable<TConstraint> Intersect(TSolver solver, TAspect otherAspect);
    }
}
