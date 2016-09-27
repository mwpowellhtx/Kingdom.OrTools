using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Kingdom.Constraints
{
    using Google.OrTools.LinearSolver;

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
        /// <see cref="OptimizationProblemType.GlopLinearProgramming"/>
        /// </summary>
        /// <see cref="OptimizationProblemType.GlopLinearProgramming"/>
        protected const OptimizationProblemType DefaultProblemType = OptimizationProblemType.GlopLinearProgramming;

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
            get { yield return LinearResultStatus.Optimal; }
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
            var valuesByName = (IDictionary<string, object>) values;
            var problemByName = (IDictionary<string, object>) problem;
            foreach (var key in problemByName.Keys)
            {
                var value = problemByName[key] as Variable;
                if (value == null) continue;
                valuesByName[key] = value.SolutionValue();
            }
            return values;
        }

        /// <summary>
        /// Receives the <paramref name="solution"/> and <paramref name="resultStatus"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="resultStatus"></param>
        /// <param name="solution"></param>
        /// <param name="problem"></param>
        protected abstract void ReceiveSolution(Solver solver, LinearResultStatus resultStatus,
            TSolution solution, dynamic problem);

        /// <summary>
        /// Try to Resolve the <see cref="Solver"/> given
        /// <see cref="ProblemSolverBase.ModelName"/> and <see cref="_problemType"/>.
        /// </summary>
        /// <returns></returns>
        public override bool TryResolve()
        {
            using (var solver = new Solver(ModelName, _problemType.ForSolver()))
            {
                PrepareVariables(solver);

                PrepareConstraints(solver);

                PrepareObjective(solver);

                var parameters = BuildParameters(GetParameters());

                // TODO: TBD: what does "result" mean?
                var resultStatus = (parameters == null
                    ? solver.Solve()
                    : solver.Solve(parameters)).FromSolver();

                var verified = VerifySolution(solver, resultStatus);

                if (verified)
                {
                    ReceiveSolution(solver, resultStatus, _getSolution(Problem), Problem);
                }

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

    /// <summary>
    /// Framework exposure of the <see cref="Solver"/> programming types.
    /// </summary>
    public enum OptimizationProblemType
    {
        /// <summary>
        /// COIN-OR linear programming.
        /// </summary>
        /// <see cref="!:http://www.coin-or.org/" />
        ClpLinearProgramming,

        /// <summary>
        /// GNU Linear Programming Kit Linear Programming (LP).
        /// </summary>
        /// <see cref="!:http://www.gnu.org/software/glpk/" />
        GlpkLinearProgramming,

        /// <summary>
        /// Google Linear Optimization or Linear Programming.
        /// </summary>
        /// <see cref="!:http://developers.google.com/optimization/lp/glop" />
        GlopLinearProgramming,

        /// <summary>
        /// SCIP mixed integer programming.
        /// </summary>
        /// <see cref="!:http://scip.zib.de/" />
        [Obsolete("Obsolete in its current form without third part inclusion and custom build.")]
        ScipMixedIntegerProgramming,

        /// <summary>
        /// GNU Linear Programming Kit Mixed Integer Programming (MIP).
        /// </summary>
        /// <see cref="!:http://www.gnu.org/software/glpk/" />
        GlpkMixedIntegerProgramming,

        /// <summary>
        /// COIN-OR Branch and Cut mixed integer programming. 
        /// </summary>
        /// <see cref="!:http://projects.coin-or.org/Cbc" />
        CbcMixedIntegerProgramming
    }

    /// <summary>
    /// Represents language level representation of the ResultStatus types.
    /// </summary>
    public enum LinearResultStatus
    {
        /// <summary>
        /// OPTIMAL
        /// </summary>
        Optimal,

        /// <summary>
        /// FEASIBLE
        /// </summary>
        Feasible,

        /// <summary>
        /// INFEASIBLE
        /// </summary>
        Infeasible,

        /// <summary>
        /// UNBOUNDED
        /// </summary>
        Unbounded,

        /// <summary>
        /// ABNORMAL
        /// </summary>
        Abnormal,

        /// <summary>
        /// MODEL_INVALID
        /// </summary>
        ModelInvalid,

        /// <summary>
        /// NOT_SOLVED
        /// </summary>
        NotSolved
    }

    /// <summary>
    /// Solver extension methods.
    /// </summary>
    internal static class SolverExtensionMethods
    {
        /// <summary>
        /// Returns the <see cref="Solver"/> <paramref name="problamType"/> for use.
        /// </summary>
        /// <param name="problamType"></param>
        /// <returns></returns>
        internal static int ForSolver(this OptimizationProblemType problamType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (problamType)
            {
                case OptimizationProblemType.GlopLinearProgramming:
                    return Solver.GLOP_LINEAR_PROGRAMMING;

                case OptimizationProblemType.ClpLinearProgramming:
                    return Solver.CLP_LINEAR_PROGRAMMING;

                case OptimizationProblemType.CbcMixedIntegerProgramming:
                    return Solver.CBC_MIXED_INTEGER_PROGRAMMING;

                // TODO: TBD: http://www.lia.disi.unibo.it/Staff/MicheleLombardi/or-tools-doc/reference_manual/or-tools/src/linear_solver/linear__solver_8h-source.html#l00168
                case OptimizationProblemType.GlpkLinearProgramming:
                case OptimizationProblemType.GlpkMixedIntegerProgramming:
                // TODO: TBD: is in at least one mirror of the software source, but probably not in the current build

                default:
                    var message = string.Format("{0} is not currently implemented", problamType);
                    throw new ArgumentException(message, "problamType");
            }
        }

        private static readonly Lazy<IDictionary<int, LinearResultStatus>> LazyResultStatuses
            = new Lazy<IDictionary<int, LinearResultStatus>>(
                () => new Dictionary<int, LinearResultStatus>
                {
                    {Solver.OPTIMAL, LinearResultStatus.Optimal},
                    {Solver.FEASIBLE, LinearResultStatus.Feasible},
                    {Solver.INFEASIBLE, LinearResultStatus.Infeasible},
                    {Solver.ABNORMAL, LinearResultStatus.Abnormal},
                    {Solver.NOT_SOLVED, LinearResultStatus.NotSolved},
                    {Solver.UNBOUNDED, LinearResultStatus.Unbounded}
                });

        private static IDictionary<int, LinearResultStatus> ResultStatuses
        {
            get { return LazyResultStatuses.Value; }
        }

        /// <summary>
        /// Returns the <paramref name="resultStatus"/> in terms of <see cref="LinearResultStatus"/>.
        /// </summary>
        /// <param name="resultStatus"></param>
        /// <returns></returns>
        internal static LinearResultStatus FromSolver(this int resultStatus)
        {
            if (ResultStatuses.ContainsKey(resultStatus))
                return ResultStatuses[resultStatus];

            var message = string.Format("Result status {0} not found", resultStatus);
            throw new ArgumentException(message, "resultStatus");
        }
    }
}
