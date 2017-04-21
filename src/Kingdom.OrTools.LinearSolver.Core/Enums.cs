using System;
using System.Collections.Generic;

namespace Kingdom.OrTools.LinearSolver
{
    using static LinearResultStatus;
    using static OptimizationProblemType;
    using static Google.OrTools.LinearSolver.Solver;

    /// <summary>
    /// Framework exposure of the <see cref="Google.OrTools.LinearSolver.Solver"/> programming types.
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
        /// <see cref="OPTIMAL"/>
        /// </summary>
        Optimal,

        /// <summary>
        /// <see cref="FEASIBLE"/>
        /// </summary>
        Feasible,

        /// <summary>
        /// <see cref="INFEASIBLE"/>
        /// </summary>
        Infeasible,

        /// <summary>
        /// <see cref="UNBOUNDED"/>
        /// </summary>
        Unbounded,

        /// <summary>
        /// <see cref="ABNORMAL"/>
        /// </summary>
        Abnormal,

        /// <summary>
        /// MODEL_INVALID
        /// </summary>
        ModelInvalid,

        /// <summary>
        /// <see cref="NOT_SOLVED"/>
        /// </summary>
        NotSolved
    }

    /// <summary>
    /// Solver extension methods.
    /// </summary>
    internal static class SolverExtensionMethods
    {
        private static readonly Lazy<IDictionary<OptimizationProblemType, int>> LazyOptimizationProblemTypes
            = new Lazy<IDictionary<OptimizationProblemType, int>>(
                () => new Dictionary<OptimizationProblemType, int>
                {
                    {GlopLinearProgramming, GLOP_LINEAR_PROGRAMMING},
                    {ClpLinearProgramming, CLP_LINEAR_PROGRAMMING},
                    {CbcMixedIntegerProgramming, CBC_MIXED_INTEGER_PROGRAMMING},
                    //// TODO: TBD: http://www.lia.disi.unibo.it/Staff/MicheleLombardi/or-tools-doc/reference_manual/or-tools/src/linear_solver/linear__solver_8h-source.html#l00168
                    //{GlpkLinearProgramming, 0},
                    //{GlpkLinearProgramming, 0},
                });

        /// <summary>
        /// Returns the <see cref="Google.OrTools.LinearSolver.Solver"/> <paramref name="value"/>
        /// for use.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int ForSolver(this OptimizationProblemType value)
        {
            try
            {
                return LazyOptimizationProblemTypes.Value[value];
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Optimization problem type '{value}' is not currently implemented",
                    nameof(value), ex);
            }
        }

        private static readonly Lazy<IDictionary<int, LinearResultStatus>> LazyResultStatuses
            = new Lazy<IDictionary<int, LinearResultStatus>>(
                () => new Dictionary<int, LinearResultStatus>
                {
                    {OPTIMAL, Optimal},
                    {FEASIBLE, Feasible},
                    {INFEASIBLE, Infeasible},
                    {ABNORMAL, Abnormal},
                    {NOT_SOLVED, NotSolved},
                    {UNBOUNDED, Unbounded}
                });

        /// <summary>
        /// Returns the <paramref name="value"/> in terms of <see cref="LinearResultStatus"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static LinearResultStatus FromSolver(this int value)
        {
            try
            {
                return LazyResultStatuses.Value[value];
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Result status '{value}' not found", nameof(value), ex);
            }
        }
    }
}
