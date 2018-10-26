namespace Kingdom.OrTools.LinearSolver
{
    using Google.OrTools.LinearSolver;

    /// <summary>
    /// Linear Problem Solver base class. Not No Solution in the sense that the problem cannot
    /// be solved. But rather, No Solution will be reported.
    /// </summary>
    /// <typeparam name="TProblemSolver"></typeparam>
    /// <remarks>Not sure why someone would not want a Solution delivered with their Problem,
    /// but this will do that much for you.</remarks>
    /// <inheritdoc />
    public abstract class OrLinearProblemSolverBase<TProblemSolver> : OrLinearProblemSolverBase<TProblemSolver, bool>
        where TProblemSolver : OrLinearProblemSolverBase<TProblemSolver>
    {
        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="problemType"></param>
        /// <inheritdoc />
        protected OrLinearProblemSolverBase(string modelName, OptimizationProblemType problemType = DefaultProblemType)
            : base(modelName, p => true, problemType)
        {
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Receives the <paramref name="resultStatus"/> and <paramref name="problem"/>.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="resultStatus"></param>
        /// <param name="problem"></param>
        protected abstract void ReceiveSolution(Solver solver, LinearResultStatus resultStatus, dynamic problem);
    }
}
