using System;

namespace Kingdom.OrTools.ConstraintSolver
{
    /// <summary>
    /// <see cref="ISearchAgent"/> extension methods.
    /// </summary>
    public static class SearchAgentExtensionMethods
    {
        // "Creating" a new SearchAgent here was not buying us much that we shouldn't just create it when it was called.

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
