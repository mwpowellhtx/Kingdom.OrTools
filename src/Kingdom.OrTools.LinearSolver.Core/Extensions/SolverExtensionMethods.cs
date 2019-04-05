using System;
using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using IProblemTypeDictionary = IDictionary<LinearOptimizationProblemType, Solver.OptimizationProblemType>;
    using IResultStatusDictionary = IDictionary<Solver.ResultStatus, LinearResultStatus>;
    using LinearProblemType = LinearOptimizationProblemType;
    using ProblemType = Solver.OptimizationProblemType;
    using ProblemTypeDictionary = Dictionary<LinearOptimizationProblemType, Solver.OptimizationProblemType>;
    using ResultStatus = Solver.ResultStatus;
    using ResultStatusDictionary = Dictionary<Solver.ResultStatus, LinearResultStatus>;
    using static Solver.ResultStatus;
    using static Solver.OptimizationProblemType;
    using static LinearOptimizationProblemType;
    using static LinearResultStatus;

    /// <summary>
    /// Solver extension methods.
    /// </summary>
    internal static class SolverExtensionMethods
    {
        // TODO: TBD: we were using dictionaries to map the integer values...
        // TODO: TBD: but with the advent of the proper enumerations, we might be better off casting to integer and then to the target enum type...
        private static IProblemTypeDictionary _problemTypes;

        private static IProblemTypeDictionary ProblemTypes
            => _problemTypes ?? (_problemTypes = new ProblemTypeDictionary
            {
                {GlopLinearProgramming, GLOP_LINEAR_PROGRAMMING},
                {ClpLinearProgramming, CLP_LINEAR_PROGRAMMING},
                {CbcMixedIntegerProgramming, CBC_MIXED_INTEGER_PROGRAMMING},
                //// TODO: TBD: http://www.lia.disi.unibo.it/Staff/MicheleLombardi/or-tools-doc/reference_manual/or-tools/src/linear_solver/linear__solver_8h-source.html#l00168
                // ReSharper disable once CommentTypo
                //{GlpkLinearProgramming, 0},
                // ReSharper disable once CommentTypo
                //{GlpkLinearProgramming, 0},
            });

        /// <summary>
        /// Returns the <see cref="Solver"/> <paramref name="value"/>
        /// for use.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ProblemType ForSolver(this LinearProblemType value)
        {
            try
            {
                return ProblemTypes[value];
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Optimization problem type '{value}' is not currently implemented",
                    nameof(value), ex);
            }
        }

        private static IResultStatusDictionary _resultStatuses;

        private static IResultStatusDictionary ResultStatuses
            => _resultStatuses ?? (_resultStatuses = new ResultStatusDictionary
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
        internal static LinearResultStatus FromSolver(this ResultStatus value)
        {
            try
            {
                return ResultStatuses[value];
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Result status '{value}' not found", nameof(value), ex);
            }
        }
    }
}
