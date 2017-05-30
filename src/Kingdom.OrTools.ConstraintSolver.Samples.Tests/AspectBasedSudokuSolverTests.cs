using System;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using NUnit.Framework;
    using OrTools.Samples;
    using Sudoku;

    /// <summary>
    /// 
    /// </summary>
    public class AspectBasedSudokuSolverTests : TestFixtureBase
    {
        /// <summary>
        /// Verifies that the aspect yields a solution.
        /// </summary>
        [Test]
        public void Verify_that_aspect_yields_solution()
        {
            using (var ps = new AspectBasedSudokuProblemSolver("Sudoku Problem Solver"))
            {
                Assert.That(ps.TryResolve());
                Assert.That(ps.Solution, Is.Not.Null);
                ps.Solution.PrettyPrint(Console.Out);
            }
        }
    }
}
