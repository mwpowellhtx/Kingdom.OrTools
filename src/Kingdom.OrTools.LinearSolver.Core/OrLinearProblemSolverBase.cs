using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Kingdom.OrTools.LinearSolver
{
    using Google.OrTools.LinearSolver;
    using MpParamSpec = Tuple<MpParamDoubleParam, double>;
    using static LinearResultStatus;
    using static LinearOptimizationProblemType;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="problem"></param>
    /// <param name="value"></param>
    public delegate void ProblemComponentSetter<in T>(dynamic problem, T value);

    /// <summary>
    /// Linear Problem Solver base class.
    /// </summary>
    /// <typeparam name="TProblemSolver"></typeparam>
    /// <typeparam name="TSolution"></typeparam>
    /// <inheritdoc cref="ProblemSolverBase{TSolver,TSource,TVariable,TConstraint}"/>
    public abstract class OrLinearProblemSolverBase<TProblemSolver, TSolution>
        : ProblemSolverBase<Solver, Solver, Variable, Constraint>
            , IOrLinearProblemSolver<TProblemSolver>
        where TProblemSolver : OrLinearProblemSolverBase<TProblemSolver, TSolution>
    {
        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// <see cref="double.NegativeInfinity"/>
        /// </summary>
        protected const double NegativeInfinity = double.NegativeInfinity;

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// <see cref="double.PositiveInfinity"/>
        /// </summary>
        protected const double PositiveInfinity = double.PositiveInfinity;

        /// <summary>
        /// Gets the ProblemType.
        /// </summary>
        private LinearOptimizationProblemType ProblemType { get; }

        /// <summary>
        /// <see cref="GlopLinearProgramming"/>
        /// </summary>
        /// <see cref="GlopLinearProgramming"/>
        protected const LinearOptimizationProblemType DefaultProblemType = GlopLinearProgramming;

        /// <summary>
        /// Delegate responsible for
        /// <see cref="OrLinearProblemSolverBase{TProblemSolver,TSolution}.GetSolution"/>.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public delegate TSolution GetSolutionDelegate(dynamic problem);

        /// <summary>
        /// Gets the Solution Getter delegate.
        /// </summary>
        private GetSolutionDelegate GetSolution { get; }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="getSolution"></param>
        /// <param name="problemType"></param>
        /// <inheritdoc />
        protected OrLinearProblemSolverBase(string modelName, GetSolutionDelegate getSolution
            , LinearOptimizationProblemType problemType = DefaultProblemType)
            : base(modelName)
        {
            GetSolution = getSolution;
            ProblemType = problemType;
        }

        /// <summary>
        /// Dynamic Problem object.
        /// </summary>
        protected readonly dynamic Problem = new ExpandoObject();

        // TODO: TBD: moving away from the "Prepare" idiom quite as much in favor of a more simplified "Get" pattern...

        /// <summary>
        /// Prepares the Constraints for the <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        protected abstract void PrepareConstraints(Solver solver);

        /// <summary>
        /// Prepares the Objective for the <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        protected abstract void PrepareObjective(Solver solver);

        /// <summary>
        /// Gets a set of AcceptableResultStatuses.
        /// </summary>
        protected virtual IEnumerable<LinearResultStatus> AcceptableResultStatuses
        {
            get { yield return Optimal; }
        }

        /// <summary>
        /// Verifies the <paramref name="solver"/> Solution.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="resultStatus"></param>
        /// <returns></returns>
        protected virtual bool VerifySolution(Solver solver, LinearResultStatus resultStatus)
        {
            return AcceptableResultStatuses.Contains(resultStatus);
        }

        /// <summary>
        /// Returns the <see cref="MPSolverParameters"/> in <see cref="IEnumerable{Double}"/> form.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<MpParamSpec> GetMpParamSpecs()
        {
            yield break;
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Sets the <see cref="Problem"/> <paramref name="value"/> via the <paramref name="setter"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected T SetProblemComponent<T>(T value, ProblemComponentSetter<T> setter)
        {
            setter(Problem, value);
            return value;
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Returns the <see cref="MpParamSpec"/> corresponding to the <paramref name="param"/>
        /// and <paramref name="value"/>.
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static MpParamSpec CreateMpParamPair(MpParamDoubleParam param, double value)
            => Tuple.Create(param, value);

        // TODO: TBD: taking LinearSolver assembly(ies) out of the build configuration for the time being until a usage/migration path can be established...
        /// <summary>
        /// Builds the <see cref="MPSolverParameters"/> instance given a set of
        /// <see cref="MpParamSpec"/> pairs.
        /// </summary>
        /// <param name="specs"></param>
        /// <returns></returns>
        protected virtual MPSolverParameters BuildParameters(params MpParamSpec[] specs)
        {
            var parameters = new MPSolverParameters();
            foreach (var (spec, value) in specs)
            {
                parameters.SetDoubleParam(spec.ForSolver(), value);
            }

            ClrCreatedObjects.Add(parameters);
            return parameters;
        }

        /// <summary>
        /// Returns the <see cref="Variable"/> values described in the <see cref="ExpandoObject"/>
        /// <paramref name="problem"/>. Leaves other details such as <see cref="Constraint"/> or
        /// <see cref="Objective"/> out of the report.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        protected static dynamic GetSolutionValues(dynamic problem)
        {
            dynamic values = new ExpandoObject();

            // We need to see the problem as a Dictionary instead of Dynamic for this to work.
            var problemByName = (IDictionary<string, object>) problem;

            // Build out the problem values by aggregation instead.
            problemByName.Keys.Where(k => problemByName[k] is Variable)
                .Select(k => (Variable) problemByName[k])
                .Aggregate((IDictionary<string, object>) values, (g, v) =>
                {
                    // We need to see the values as a Dictionary instead of Dynamic for this to work.
                    g[v.Name()] = v.SolutionValue();
                    return g;
                });

            return values;
        }

        /// <inheritdoc />
        public class SolutionEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the <see cref="Solver.NumVariables"/> modeled.
            /// </summary>
            public int VariableCount { get; }

            /// <summary>
            /// Gets the <see cref="Solver.NumConstraints"/> modeled.
            /// </summary>
            public int ConstraintCount { get; }

            /// <summary>
            /// Gets the <see cref="Solver.Solve()"/> result.
            /// </summary>
            public LinearResultStatus ResultStatus { get; }

            /// <summary>
            /// Gets the Solution calculated from the model.
            /// </summary>
            public TSolution Solution { get; }

            /// <summary>
            /// Gets the <see cref="ExpandoObject"/> SolutionValues.
            /// </summary>
            public dynamic SolutionValues { get; }

            /// <summary>
            /// Internal Constructor
            /// </summary>
            /// <param name="solver"></param>
            /// <param name="resultStatus"></param>
            /// <param name="solution"></param>
            /// <param name="solutionValues"></param>
            /// <inheritdoc />
            public SolutionEventArgs(Solver solver, LinearResultStatus resultStatus, TSolution solution
                , dynamic solutionValues)
            {
                VariableCount = solver.NumVariables();
                ConstraintCount = solver.NumConstraints();
                ResultStatus = resultStatus;
                Solution = solution;
                SolutionValues = solutionValues;
            }
        }

        /// <summary>
        /// Solved event handler.
        /// </summary>
        public event EventHandler<SolutionEventArgs> Solved;

        /// <summary>
        /// Raised the Solved <paramref name="e"/> event arguments.
        /// </summary>
        /// <param name="e"></param>
        protected void OnSolved(SolutionEventArgs e)
        {
            Solved?.Invoke(this, e);
        }

        /// <summary>
        /// Receives the <paramref name="solution"/> and <paramref name="resultStatus"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="resultStatus"></param>
        /// <param name="solution"></param>
        /// <param name="problem"></param>
        protected void ReceiveSolution(Solver solver, LinearResultStatus resultStatus,
            TSolution solution, dynamic problem)
        {
            OnSolved(new SolutionEventArgs(solver, resultStatus, solution, GetSolutionValues(problem)));
        }

        /// <summary>
        /// Try to Resolve the <see cref="Solver"/> given
        /// <see cref="ProblemSolverBase{TSolver,TSource,TVariable,TConstraint}.ModelName"/> and
        /// <see cref="ProblemType"/>.
        /// </summary>
        /// <returns></returns>
        /// <inheritdoc />
        public override bool TryResolve()
        {
            var e = EventArgs.Empty;

            using (Solver = new Solver(ModelName, ProblemType.ForSolver()))
            {
                var solver = Solver;

                // TODO: TBD: consider what sort of Linear Search Agent, fluent configuration, could be done here...
                // TODO: TBD: do anything further with the variables here?
                // ReSharper disable once UnusedVariable
                var variables = Variables.Select(x => x.TrackClrObject(this)).ToArray();

                PrepareConstraints(solver);

                PrepareObjective(solver);

                var parameters = BuildParameters(GetMpParamSpecs().ToArray());

                OnResolving(e);

                // TODO: TBD: what does "result" mean?
                var resultStatus = (parameters == null ? solver.Solve() : solver.Solve(parameters)).FromSolver();

                var verified = VerifySolution(solver, resultStatus);

                if (verified)
                {
                    // TODO: TBD: should rethink this along the same lines as with Constraint Solver...
                    ReceiveSolution(solver, resultStatus, GetSolution(Problem), Problem);
                }

                OnResolved(e);

                return verified;
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
        }
    }
}
