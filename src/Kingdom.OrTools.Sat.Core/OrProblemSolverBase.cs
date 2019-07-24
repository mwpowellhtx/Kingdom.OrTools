using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat
{
    using Google.OrTools.Sat;
    using Parameters;
    using static Parameters.Characters;
    using static String;
    using static CpSolverStatus;
    using SolverStatus = Google.OrTools.Sat.CpSolverStatus;

    /// <summary>
    /// or-tools-based Sat Constraint Programming problem solver.
    /// </summary>
    /// <inheritdoc cref="ProblemSolverBase{TSolver,TSource,TVariable,TConstraint}"/>
    public abstract class OrProblemSolverBase : ProblemSolverBase<CpSolver, CpModel, IntVar, Constraint>, IOrProblemSolver
    {
        private CpModel _source;

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        /// <inheritdoc />
        public override CpModel Source => _source ?? (_source = new CpModel { });

        /// <summary>
        /// The default set of results includes only <see cref="Optimal"/>.
        /// Override to specify a more robust set of Expected Results.
        /// </summary>
        /// <inheritdoc />
        public virtual IEnumerable<CpSolverStatus> ExpectedResults
        {
            get { yield return Optimal; }
        }

        /// <summary>
        /// Gets the <see cref="IntervalVar"/> Intervals for the Problem Solver.
        /// </summary>
        protected virtual IEnumerable<IntervalVar> Intervals
        {
            get { yield break; }
        }

        /// <summary>
        /// Gets the <see cref="Constraint"/> instances for the Problem Solver.
        /// </summary>
        protected virtual IEnumerable<Constraint> Constraints
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public virtual IParameterCollection Parameters { get; } = new ParameterCollection();

        /// <summary>
        /// Furnishes a <see cref="string"/> representation of the <see cref="Parameters"/>
        /// <see cref="IParameter"/> <see cref="ICollection{T}"/>. Override in the event there
        /// is any further work you want to accomplish using the represented parameters.
        /// </summary>
        /// <see cref="Space"/>
        /// <see cref="Google.OrTools.Sat.CpSolver.StringParameters"/>
        protected virtual string ParametersString => $"{Parameters}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected delegate SolverStatus TryResolveCallback(CpSolver solver, CpModel source);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="source"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected bool TryResolve(CpSolver solver, CpModel source, TryResolveCallback callback)
        {
            var result = callback(solver, source).FromSolver();
            return ExpectedResults.Contains(result);
        }

        /// <summary>
        /// Prepares the <see cref="CpSolver"/>. Indirectly also prepares the
        /// <see cref="CpModel"/>, which is really the focus of the Sat based approach.
        /// </summary>
        /// <returns></returns>
        protected virtual Tuple<CpSolver, CpModel> PrepareProblemSolver()
        {
            {
                var stringParameters = ParametersString;
                // Only relay the StringParameters when we actually have them.
                Solver = IsNullOrEmpty(stringParameters)
                    ? new CpSolver()
                    : new CpSolver {StringParameters = stringParameters};
            }

            /* Truly there is nothing to convey to any Monitors, Search Agents, etc, for the Sat
             * approach. Once they are New created or Added, they are registered with the Model
             * forever and ever AMEN. */

            // ReSharper disable once UnusedVariable
            var variables = Variables.ToList();
            // ReSharper disable once UnusedVariable
            var intervals = Intervals.ToList();
            // ReSharper disable once UnusedVariable
            var constraints = Constraints.ToArray();
            // TODO: TBD: any other aspects of Solver/Model that need to be obtained?

            return Tuple.Create(Solver, Source);
        }

        private delegate bool ResolutionCallback();

        // TODO: TBD: does this "context" need to be more formalized, a "search agent", etc?
        private bool TryResolveContext(ResolutionCallback callback)
        {
            var e = EventArgs.Empty;

            OnResolving(e);

            var solved = callback();

            if (solved)
            {
                OnSolved(e);
            }

            OnResolved(e);

            return solved;
        }

        /// <inheritdoc />
        public override bool TryResolve()
        {
            var (solver, source) = PrepareProblemSolver();
            return TryResolveContext(() => TryResolve(solver, source, (x, y) => x.Solve(y)));
        }

        /// <inheritdoc />
        public virtual bool TryResolve<TCallback>(OrProblemSolverSolutionCallbackFactory<TCallback> solutionCallbackFactory)
            where TCallback : OrProblemSolverSolutionCallback
        {
            var slnCallback = solutionCallbackFactory();
            slnCallback.Callback += () => OnSolved(EventArgs.Empty);
            var (solver, source) = PrepareProblemSolver();
            return TryResolveContext(() => TryResolve(solver, source, (x, y) => x.SolveWithSolutionCallback(y, slnCallback)));
        }

        /// <inheritdoc />
        public virtual bool TryResolveAll<TCallback>(OrProblemSolverSolutionCallbackFactory<TCallback> solutionCallbackFactory)
            where TCallback : OrProblemSolverSolutionCallback
        {
            var slnCallback = solutionCallbackFactory();
            slnCallback.Callback += () => OnSolved(EventArgs.Empty);
            var (solver, source) = PrepareProblemSolver();
            return TryResolveContext(() => TryResolve(solver, source, (x, y) => x.SearchAllSolutions(y, slnCallback)));
        }

        /// <inheritdoc />
        /// <see cref="IOrProblemSolver.Solved"/>
        public event EventHandler<EventArgs> Solved;

        /// <summary>
        /// Raises the <see cref="Solved"/> event.
        /// </summary>
        /// <param name="e"></param>
        /// <see cref="IOrProblemSolver.Solved"/>
        protected virtual void OnSolved(EventArgs e) => Solved?.Invoke(this, e);
    }
}
