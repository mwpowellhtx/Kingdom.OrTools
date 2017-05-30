using System;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    internal class ProblemSolverImplementation
    {
        // TODO: TBD: may need/want additional hooks to control policies such as "get solver seed"...
        /// <summary>
        /// Returns a seed for the <see cref="Solver"/> to
        /// <see cref="Google.OrTools.ConstraintSolver.Solver.ReSeed"/>.
        /// The default behavior defer to the <see cref="Random.Next()"/> method.
        /// </summary>
        /// <returns></returns>
        internal virtual int GetRandomSeed() => new Random().Next();

        /// <summary>
        /// Initializes the problem solver.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="seed"></param>
        internal virtual void ReSeed(Solver solver, int seed) => solver.ReSeed(seed);
    }
}
