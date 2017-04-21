namespace Kingdom.OrTools.ConstraintSolver
{
    /// <summary>
    /// Provides a problem solving interface for adaptation.
    /// </summary>
    /// <typeparam name="TProblemSolver"></typeparam>
    public interface IOrProblemSolver<out TProblemSolver> : IProblemSolver
        where TProblemSolver : class, IOrProblemSolver<TProblemSolver>
    {
    }
}
