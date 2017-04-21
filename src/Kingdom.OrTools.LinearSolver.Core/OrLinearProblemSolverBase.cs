using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Kingdom.OrTools.LinearSolver
{
    using Google.OrTools.LinearSolver;
    using static LinearResultStatus;
    using static OptimizationProblemType;

    /// <summary>
    /// Linear Problem Solver base class.
    /// </summary>
    /// <typeparam name="TProblemSolver"></typeparam>
    /// <typeparam name="TSolution"></typeparam>
    public abstract class OrLinearProblemSolverBase<TProblemSolver, TSolution>
        : ProblemSolverBase, IOrLinearProblemSolver<TProblemSolver>
        where TProblemSolver : OrLinearProblemSolverBase<TProblemSolver, TSolution>
    {
        /// <summary>
        /// <see cref="Double.NegativeInfinity"/>
        /// </summary>
        protected const double NegativeInfinity = double.NegativeInfinity;

        /// <summary>
        /// <see cref="Double.PositiveInfinity"/>
        /// </summary>
        protected const double PositiveInfinity = double.PositiveInfinity;

        /// <summary>
        /// ProblemType backing field.
        /// </summary>
        private readonly OptimizationProblemType _problemType;

        /// <summary>
        /// <see cref="GlopLinearProgramming"/>
        /// </summary>
        /// <see cref="GlopLinearProgramming"/>
        protected const OptimizationProblemType DefaultProblemType = GlopLinearProgramming;

        /// <summary>
        /// Delegate responsible for
        /// <see cref="OrLinearProblemSolverBase{TProblemSolver,TSolution}._getSolution"/>.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public delegate TSolution GetSolutionDelegate(dynamic problem);

        private readonly GetSolutionDelegate _getSolution;

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="getSolution"></param>
        /// <param name="problemType"></param>
        protected OrLinearProblemSolverBase(string modelName,
            GetSolutionDelegate getSolution,
            OptimizationProblemType problemType = DefaultProblemType)
            : base(modelName)
        {
            _getSolution = getSolution;
            _problemType = problemType;
        }

        /// <summary>
        /// Dynamic Problem object.
        /// </summary>
        protected readonly dynamic Problem = new ExpandoObject();

        /// <summary>
        /// Prepares the Variables for the <paramref name="solver"/>.
        /// </summary>
        /// <param name="solver"></param>
        protected abstract void PrepareVariables(Solver solver);

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
        protected virtual IEnumerable<double> GetParameters()
        {
            yield break;
        }

        /// <summary>
        /// Sets the <see cref="Problem"/> <paramref name="value"/> via the <paramref name="setter"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected T SetProblemComponent<T>(T value, Action<dynamic, T> setter)
        {
            setter(Problem, value);
            return value;
        }

        private MPSolverParameters BuildParameters(IEnumerable<double> values)
        {
            var parameters = new MPSolverParameters();
            // ReSharper disable PossibleMultipleEnumeration
            for (var i = 0; i < values.Count(); i++)
            {
                parameters.SetDoubleParam(i, values.ElementAt(i));
                // ReSharper enable PossibleMultipleEnumeration
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

        /// <summary>
        /// SolutionEventArgs event arguments.
        /// </summary>
        public class SolutionEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the <see cref="Solver.NumVariables"/> modeled.
            /// </summary>
            public int VariableCount { get; private set; }

            /// <summary>
            /// Gets the <see cref="Solver.NumConstraints"/> modeled.
            /// </summary>
            public int ConstraintCount { get; private set; }

            /// <summary>
            /// Gets the <see cref="Solver.Solve()"/> result.
            /// </summary>
            public LinearResultStatus ResultStatus { get; private set; }

            /// <summary>
            /// Gets the Solution calculated from the model.
            /// </summary>
            public TSolution Solution { get; private set; }

            /// <summary>
            /// Gets the <see cref="ExpandoObject"/> SolutionValues.
            /// </summary>
            public dynamic SolutionValues { get; private set; }

            /// <summary>
            /// Internal Constructor
            /// </summary>
            /// <param name="solver"></param>
            /// <param name="resultStatus"></param>
            /// <param name="solution"></param>
            /// <param name="solutionValues"></param>
            public SolutionEventArgs(Solver solver, LinearResultStatus resultStatus,
                TSolution solution, dynamic solutionValues)
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
        /// <see cref="ProblemSolverBase.ModelName"/> and <see cref="_problemType"/>.
        /// </summary>
        /// <returns></returns>
        public override bool TryResolve()
        {
            var e = EventArgs.Empty;

            using (var solver = new Solver(ModelName, _problemType.ForSolver()))
            {
                // TODO: TBD: consider what sort of Linear Search Agent, fluent configuration, could be done here...
                PrepareVariables(solver);

                PrepareConstraints(solver);

                PrepareObjective(solver);

                var parameters = BuildParameters(GetParameters());

                OnResolving(e);

                // TODO: TBD: what does "result" mean?
                var resultStatus = (parameters == null
                    ? solver.Solve()
                    : solver.Solve(parameters)).FromSolver();

                var verified = VerifySolution(solver, resultStatus);

                if (verified)
                {
                    // TODO: TBD: should rethink this along the same lines as with Constraint Solver...
                    ReceiveSolution(solver, resultStatus, _getSolution(Problem), Problem);
                }

                OnResolved(e);

                return verified;
            }
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
