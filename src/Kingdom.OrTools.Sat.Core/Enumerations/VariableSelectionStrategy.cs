using Google.OrTools.Sat;

namespace Kingdom.OrTools.Sat
{
    using SelectionStrategy = DecisionStrategyProto.Types.VariableSelectionStrategy;

    /// <summary>
    /// We adapt the <see cref="SelectionStrategy"/> into the framework so it feels more natural
    /// to work with.
    /// </summary>
    /// <see cref="EnumerationExtensionMethods.ForSolver(VariableSelectionStrategy)"/>
    public enum VariableSelectionStrategy
    {
        /// <summary>
        /// <see cref="SelectionStrategy.ChooseFirst"/>
        /// </summary>
        ChooseFirst = SelectionStrategy.ChooseFirst,

        /// <summary>
        /// <see cref="SelectionStrategy.ChooseLowestMin"/>
        /// </summary>
        ChooseLowestMin = SelectionStrategy.ChooseLowestMin,

        /// <summary>
        /// <see cref="SelectionStrategy.ChooseHighestMax"/>
        /// </summary>
        ChooseHighestMax = SelectionStrategy.ChooseHighestMax,

        /// <summary>
        /// <see cref="SelectionStrategy.ChooseMinDomainSize"/>
        /// </summary>
        ChooseMinDomainSize = SelectionStrategy.ChooseMinDomainSize,

        /// <summary>
        /// <see cref="SelectionStrategy.ChooseMaxDomainSize"/>
        /// </summary>
        ChooseMaxDomainSize = SelectionStrategy.ChooseMaxDomainSize
    }
}
