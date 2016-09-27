namespace Kingdom.Constraints
{
    public interface IOrLinearProblemSolver<TProblemSolver>
        where TProblemSolver : IOrLinearProblemSolver<TProblemSolver>
    {
    }
}
