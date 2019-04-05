namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provides a problem solving interface for adaptation.
    /// </summary>
    /// <inheritdoc />
    public interface IOrProblemSolver : IProblemSolver<Solver>
    {
    }

    /// <summary>
    /// <typeparamref name="TAspect"/> based
    /// <see cref="IProblemSolver{Solver,Solver,IntVar,Constraint,TAspect}"/>.
    /// </summary>
    /// <typeparam name="TAspect"></typeparam>
    /// <inheritdoc cref="IProblemSolver{TSolver,TSource,TVariable,TConstraint,TAspect}"/>
    public interface IOrProblemSolver<TAspect> : IProblemSolver<Solver, Solver, IntVar, Constraint, TAspect>, IOrProblemSolver
        where TAspect : IProblemSolverAspect<Solver, Solver, IntVar, Constraint, TAspect>
    {
    }
}
