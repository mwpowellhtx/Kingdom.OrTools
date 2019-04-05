namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;
    using VariableChooser = Google.OrTools.ConstraintSolver.LongLongToLong;

    /// <summary>
    /// Provides some helpful <see cref="Solver"/> extension methods.
    /// </summary>
    public static class ConstraintSolverExtensionMethods
    {
        /// <summary>
        /// Provides a helpful C-Sharp friendly extension method for <see
        /// cref="Solver.MakePhase(IntVarVector, int, int)"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variables"></param>
        /// <param name="varStrategy"></param>
        /// <param name="valStrategy"></param>
        /// <returns></returns>
        public static DecisionBuilder MakePhase(this Solver solver, IntVarVector variables,
            IntVarStrategy varStrategy, IntValueStrategy valStrategy)
            => solver.MakePhase(variables, varStrategy.ToInt(), valStrategy.ToInt());

        /// <summary>
        /// Provides a helpful C-Sharp friendly extension method for <see
        /// cref="Solver.MakePhase(IntVarVector, VariableChooser, int)"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variables"></param>
        /// <param name="varChooser"></param>
        /// <param name="evalStrategy"></param>
        /// <returns></returns>
        public static DecisionBuilder MakePhase(this Solver solver, IntVarVector variables,
            VariableChooser varChooser, EvaluatorStrategy evalStrategy)
            => solver.MakePhase(variables, varChooser, evalStrategy.ToInt());
    }
}
