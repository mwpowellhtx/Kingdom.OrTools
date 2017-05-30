using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Sudoku problem solver.
    /// </summary>
    /// <see cref="!:http://github.com/google/or-tools/blob/master/examples/python/sudoku.py"/>
    public class SudokuProblemSolver : OrProblemSolverBase
    {
        /// <summary>
        /// Gets the Puzzle.
        /// </summary>
        private ISudokuPuzzle Puzzle { get; }

        /// <summary>
        /// Gets the Solution.
        /// </summary>
        public ISudokuPuzzle Solution { get; private set; } = new SudokuPuzzle();

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="puzzle"></param>
        public SudokuProblemSolver(ISudokuPuzzle puzzle)
            : base(@"Sudoku Solver")
        {
            Puzzle = puzzle;
        }

        /// <summary>
        /// Returns a made cell variable.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public IntVar MakeCell(Solver solver, int row, int column)
            => solver.MakeIntVar(0, 9, $@"SudokuPuzzle[{row}, {column}]");

        /// <summary>
        /// Cells backing field.
        /// </summary>
        private readonly IntVar[,] _cells = new IntVar[9, 9];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        protected sealed override IEnumerable<IntVar> GetVariables(Solver solver)
        {
            return from cell in ((SudokuPuzzle) Puzzle)
                .OrderBy(c => c.Key.Row).ThenBy(c => c.Key.Column)
                select cell.Key
                into key
                let i = key.Row
                let j = key.Column
                select _cells[i, j] = MakeCell(solver, i, j).TrackClrObject(this);
        }

        /// <summary>
        /// Makes an initial constraint on each cell if possible.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private IEnumerable<Constraint> MakeInitialConstraints(Solver solver, IntExpr variable, int value)
        {
            // TODO: TBD: this one may change for partially solved puzzles.
            yield return solver.MakeBetweenCt(variable, 1, 9);

            if (value.TrySolvedValue())
            {
                yield return variable == value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        private IEnumerable<Constraint> MakeAllDifferentConstraints(Solver solver,
            IEnumerable<IDictionary<Address, int>> groups)
        {
            var groupedCells = groups.Select(g => g.Select(
                h => _cells[h.Key.Row, h.Key.Column]).ToList()).ToArray();

            foreach (var cdiff in groupedCells
                .Select(g => solver.MakeAllDifferent(new IntVarVector(g))))
            {
                yield return cdiff;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        protected override IEnumerable<Constraint> PrepareConstraints(Solver solver)
        {
            foreach (var cell in (SudokuPuzzle) Puzzle)
            {
                var key = cell.Key;
                var row = key.Row;
                var col = key.Column;

                foreach (var c in MakeInitialConstraints(solver, _cells[row, col], Puzzle[row, col]))
                {
                    yield return c;
                }
            }

            var p = Puzzle;

            foreach (var c in MakeAllDifferentConstraints(solver, p.Rows.Concat(p.Columns).Concat(p.Blocks)))
            {
                yield return c;
            }
        }

        /// <summary>
        /// <see cref="ISearchAgent"/> ProcessVariables event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnProcessVariables(object sender, ProcessVariablesEventArgs e)
        {
            var candidate = new SudokuPuzzle();
            ISudokuPuzzle local = candidate;

            for (var row = 0; row < 9; row++)
            {
                for (var col = 0; col < 9; col++)
                {
                    local[row, col] = (int) _cells[row, col].Value();
                }
            }

            /* If we're here processing variables, it should be because we are processing the next
             * solution. However, in the event we still do not have a solution, then simply return. */

            if (!local.IsSolved) return;

            Solution = local;

            // False is the default, so only mark whether ShouldBreak when we have one.
            e.ShouldBreak = true;

            base.OnProcessVariables(sender, e);
        }
    }
}
