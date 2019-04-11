using Google.OrTools.Sat;

namespace Kingdom.OrTools.Sat
{
    using ReductionStrategy = DecisionStrategyProto.Types.DomainReductionStrategy;

    /// <summary>
    /// We adapt the <see cref="ReductionStrategy"/> into the framework so it feels more natural
    /// to work with.
    /// </summary>
    /// <see cref="EnumerationExtensionMethods.ForSolver(DomainReductionStrategy)"/>
    public enum DomainReductionStrategy
    {
        /// <summary>
        /// <see cref="ReductionStrategy.SelectMinValue"/>
        /// </summary>
        SelectMinValue = ReductionStrategy.SelectMinValue,

        /// <summary>
        /// <see cref="ReductionStrategy.SelectMaxValue"/>
        /// </summary>
        SelectMaxValue = ReductionStrategy.SelectMaxValue,

        /// <summary>
        /// <see cref="ReductionStrategy.SelectLowerHalf"/>
        /// </summary>
        SelectLowerHalf = ReductionStrategy.SelectLowerHalf,

        /// <summary>
        /// <see cref="ReductionStrategy.SelectUpperHalf"/>
        /// </summary>
        SelectUpperHalf = ReductionStrategy.SelectUpperHalf
    }
}
