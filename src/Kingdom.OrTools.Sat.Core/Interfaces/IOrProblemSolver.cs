using System;
using System.Collections.Generic;

namespace Kingdom.OrTools.Sat
{
    using Google.OrTools.Sat;
    using Parameters;

    /// <summary>
    /// Provides a problem solving interface for adaptation.
    /// </summary>
    /// <inheritdoc />
    public interface IOrProblemSolver : IProblemSolver<CpSolver, CpModel>
    {
        /// <summary>
        /// Gets the <see cref="IParameterCollection"/> Parameters instance.
        /// </summary>
        IParameterCollection Parameters { get; }

        /// <summary>
        /// Gets the Expected Results.
        /// </summary>
        IEnumerable<CpSolverStatus> ExpectedResults { get; }

        /// <summary>
        /// Tries to Resolve the Solution given a <typeparamref name="TCallback"/>.
        /// </summary>
        /// <typeparam name="TCallback"></typeparam>
        /// <param name="solutionCallbackFactory"></param>
        /// <returns></returns>
        /// <see cref="OrProblemSolverSolutionCallbackFactory{TCallback}"/>
        /// <see cref="SolutionCallback"/>
        bool TryResolve<TCallback>(OrProblemSolverSolutionCallbackFactory<TCallback> solutionCallbackFactory)
            where TCallback : OrProblemSolverSolutionCallback;

        /// <summary>
        /// Tries to Resolve All of the Solutions given a <typeparamref name="TCallback"/>.
        /// </summary>
        /// <typeparam name="TCallback"></typeparam>
        /// <param name="solutionCallbackFactory"></param>
        /// <returns></returns>
        /// <see cref="OrProblemSolverSolutionCallbackFactory{TCallback}"/>
        /// <see cref="SolutionCallback"/>
        bool TryResolveAll<TCallback>(OrProblemSolverSolutionCallbackFactory<TCallback> solutionCallbackFactory)
            where TCallback : OrProblemSolverSolutionCallback;

        /// <summary>
        /// Solved event.
        /// </summary>
        event EventHandler<EventArgs> Solved;
    }

    //// TODO: TBD: "aspect oriented" constraint solvers under consideration, but let's get this done first.
    ///// <summary>
    ///// <typeparamref name="TAspect"/> based
    ///// <see cref="IProblemSolver{Solver,Solver,IntVar,Constraint,TAspect}"/>.
    ///// </summary>
    ///// <typeparam name="TAspect"></typeparam>
    ///// <inheritdoc cref="IProblemSolver{TSolver,TSource,TVariable,TConstraint,TAspect}"/>
    //public interface IOrProblemSolver<TAspect> : IProblemSolver<CpSolver, CpModel, IntVar, Constraint, TAspect>, IOrProblemSolver
    //    where TAspect : IProblemSolverAspect<CpSolver, CpModel, IntVar, Constraint, TAspect>
    //{
    //}
}
