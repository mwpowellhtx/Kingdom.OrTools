using System;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// <see cref="ISearchAgent"/> extension methods.
    /// </summary>
    public static class SearchAgentExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        internal static ISearchAgent PrepareSearch(this Solver solver, params IntVar[] variables)
        {
            return new SearchAgent(solver, variables);
        }

        // TODO: TBD: a bit on the fence about the prospect of a "for-each-solution"; could make for an interesting API, but is it necessary?

            /// <summary>
        /// 
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ISearchAgent ForEachSolution(this ISearchAgent agent, EventHandler<ProcessVariablesEventArgs> handler)
        {
            agent.ProcessVariables += handler;
            return agent;
        }
    }
}
