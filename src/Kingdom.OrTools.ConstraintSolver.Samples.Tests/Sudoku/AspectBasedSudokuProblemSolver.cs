using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using Google.OrTools.ConstraintSolver;
    using Kingdom.OrTools.Samples.Sudoku;
    using Xunit;
    using static IntValueStrategy;
    using static IntVarStrategy;
    using static Kingdom.OrTools.Samples.Sudoku.Domain;

    public class AspectBasedSudokuProblemSolver : OrProblemSolverBase<SudokuProblemSolverAspect>
    {
        private static IEnumerable<SudokuProblemSolverAspect> GetDefaultAspects()
        {
            yield return new SudokuProblemSolverAspect();
        }

        public AspectBasedSudokuProblemSolver(string modelName)
            : base(modelName, GetDefaultAspects())
        {
        }

        protected override IEnumerable<Constraint> PrepareConstraints(Solver source)
        {
            // TODO: TBD: do we need to implement anything here?
            yield break;
        }

        /// <summary>
        /// Gets the Solution.
        /// </summary>
        public ISudokuPuzzle Solution { get; private set; } = new SudokuPuzzle();

        /// <summary>
        /// Begins a New Search corresponding with the <see cref="Solver"/> and
        /// <paramref name="agent"/>.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        /// <inheritdoc />
        protected override ISearchAgent NewSearch(ISearchAgent agent)
        {
            agent.ProcessVariables -= OnProcessVariables;
            agent.ProcessVariables += OnProcessVariables;

            // The simple strategies are okay and yield expected results. But let's keep it interesting.
            return agent.NewSearch(a => a.Solver.MakePhase(a.Variables, ChooseRandom, AssignRandomValue));
        }

        /// <summary>
        /// <see cref="ISearchAgent"/> ProcessVariables event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <inheritdoc />
        protected override void OnProcessVariables(object sender, ProcessVariablesEventArgs e)
        {
            ISudokuPuzzle candidate = new SudokuPuzzle();

            // In this case we know that there is a Single Aspect.
            var aspect = Aspects.SingleOrDefault();
            Assert.NotNull(aspect);

            for (var row = MinimumValue; row < MaximumValue; row++)
            {
                for (var col = MinimumValue; col < MaximumValue; col++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    candidate[row, col] = (int) aspect.Cells[row, col].Value();
                }
            }

            /* If we're here processing variables, it should be because we are processing the next
             * solution. However, in the event we still do not have a solution, then simply return. */

            // TODO: TBD: we really should never land here I don't think...
            if (candidate.IsSolved)
            {
                Solution = candidate;

                // False is the default, so only mark whether ShouldBreak when we have one.
                e.ShouldBreak = true;
            }

            base.OnProcessVariables(sender, e);
        }
    }
}
