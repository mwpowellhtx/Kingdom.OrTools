using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;

namespace Kingdom.OrTools.Sat
{
    using static DomainReductionStrategy;
    using static VariableSelectionStrategy;
    using static CpSolverStatus;
    using ReductionStrategy = DecisionStrategyProto.Types.DomainReductionStrategy;
    using SelectionStrategy = DecisionStrategyProto.Types.VariableSelectionStrategy;
    using SolverStatus = Google.OrTools.Sat.CpSolverStatus;

    internal static class EnumerationExtensionMethods
    {
        public static bool ContainedBy(this CpSolverStatus value, params CpSolverStatus[] values)
            => values.Contains(value);

        private static TValue ForSolver<TKey, TValue>(this TKey key
            , IDictionary<TKey, TValue> dictionary)
        {
            TValue ThrowArgumentException()
            {
                // Although this does return in theory we actually do want to throw the Exception.
                throw new ArgumentException($"Unexpected '{nameof(key)}' value '{key}'.", nameof(key));
            }

            return dictionary.TryGetValue(key, out var result) ? result : ThrowArgumentException();
        }

        private static IDictionary<DomainReductionStrategy, ReductionStrategy> DomainReductions { get; }
            = new Dictionary<DomainReductionStrategy, ReductionStrategy>
            {
                {SelectMaxValue, ReductionStrategy.SelectMaxValue},
                {SelectMinValue, ReductionStrategy.SelectMinValue},
                {SelectLowerHalf, ReductionStrategy.SelectLowerHalf},
                {SelectUpperHalf, ReductionStrategy.SelectUpperHalf}
            };

        internal static ReductionStrategy ForSolver(this DomainReductionStrategy value)
            => value.ForSolver(DomainReductions);

        private static IDictionary<VariableSelectionStrategy, SelectionStrategy> VariableSelections { get; }
            = new Dictionary<VariableSelectionStrategy, SelectionStrategy>
            {
                {ChooseFirst, SelectionStrategy.ChooseFirst},
                {ChooseLowestMin, SelectionStrategy.ChooseLowestMin},
                {ChooseHighestMax, SelectionStrategy.ChooseHighestMax},
                {ChooseMinDomainSize, SelectionStrategy.ChooseMinDomainSize},
                {ChooseMaxDomainSize, SelectionStrategy.ChooseMaxDomainSize}
            };

        internal static SelectionStrategy ForSolver(this VariableSelectionStrategy value)
            => value.ForSolver(VariableSelections);

        private static IDictionary<CpSolverStatus, SolverStatus> GetDefaultSolverStatuses()
            => new Dictionary<CpSolverStatus, SolverStatus>
            {
                {Unknown, SolverStatus.Unknown},
                {ModelInvalid, SolverStatus.ModelInvalid},
                {Feasible, SolverStatus.Feasible},
                {Infeasible, SolverStatus.Infeasible},
                {Optimal, SolverStatus.Optimal},
            };

        private static IDictionary<CpSolverStatus, SolverStatus> ForSolverStatuses { get; }
            = GetDefaultSolverStatuses();

        private static IDictionary<SolverStatus, CpSolverStatus> FromSolverStatuses { get; }
            = ForSolverStatuses.ToDictionary(item => item.Value, item => item.Key);

        internal static SolverStatus ForSolver(this CpSolverStatus value)
            => value.ForSolver(ForSolverStatuses);

        internal static CpSolverStatus FromSolver(this SolverStatus value)
            => value.ForSolver(FromSolverStatuses);
    }
}
