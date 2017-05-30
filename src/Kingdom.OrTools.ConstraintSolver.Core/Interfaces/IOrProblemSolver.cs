namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provides a problem solving interface for adaptation.
    /// </summary>
    public interface IOrProblemSolver : IProblemSolver<Solver>
    {
    }

    /// <summary>
    /// <typeparamref name="TAspect"/> based
    /// <see cref="IProblemSolver{Solver,IntVar,Constraint,TAspect}"/>.
    /// </summary>
    /// <typeparam name="TAspect"></typeparam>
    public interface IOrProblemSolver<TAspect> : IProblemSolver<Solver, IntVar, Constraint, TAspect>, IOrProblemSolver
        where TAspect : IProblemSolverAspect<Solver, IntVar, Constraint, TAspect>
    {
    }
}
