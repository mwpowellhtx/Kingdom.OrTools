using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Samples.Sudoku
{
    using Google.OrTools.Sat;
    using Kingdom.OrTools.Samples.Sudoku;
    using static Kingdom.OrTools.Samples.Sudoku.Domain;
    using static Kingdom.OrTools.Samples.Sudoku.SudokuPuzzle;
    using static CpSolverStatus;
    using RowMajorAddressTuple = Tuple<int, int>;

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
        /// Default Constructor.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <inheritdoc />
        public SudokuProblemSolver(ISudokuPuzzle puzzle)
        {
            Puzzle = puzzle;
        }

        public override IEnumerable<Sat.CpSolverStatus> ExpectedResults
        {
            get
            {
                yield return Optimal;
                yield return Feasible;
            }
        }

        /// <summary>
        /// Returns a Made <see cref="_cells"/> <see cref="IntVar"/>.
        /// </summary>
        /// <param name="source">In this case Source is a <see cref="CpModel"/>,
        /// different from the Solver <see cref="CpSolver"/>.</param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>The vocabulary has also changed from Solver Make such and such,
        /// to Source or Model New such and such.</remarks>
        public IntVar NewCell(CpModel source, int row, int column, int value)
        {
            var variableName = $"SudokuPuzzle[{row}, {column}]";

            return value.TrySolvedValue()
                ? source.NewConstant(value, variableName)
                : source.NewIntVar(MinimumValue + 1, MaximumValue, variableName);
        }

        private readonly IntVar[,] _cells = new IntVar[Size, Size];

        private IEnumerable<IntVar> _variables;

        protected override IEnumerable<IntVar> Variables
        {
            get
            {
                var source = Source;

                IEnumerable<IntVar> GetAll()
                {
                    foreach (var (key, value) in ((SudokuPuzzle) Puzzle).OrderBy(x => x.Key.Row).ThenBy(x => x.Key.Column))
                    {
                        var row = key.Row;
                        var col = key.Column;
                        yield return _cells[row, col] = NewCell(source, row, col, value);
                    }
                }

                return _variables ?? (_variables = GetAll().ToArray());
            }
        }

        private IEnumerable<Constraint> _constraints;

        protected override IEnumerable<Constraint> Constraints
        {
            get
            {
                IEnumerable<Constraint> GetAll()
                {
                    var p = Puzzle;
                    var source = Source;

                    foreach (var c in p.Rows.Concat(p.Columns).Concat(p.Blocks)
                        .Select(g =>
                            (from pair in g
                                select pair.Key
                                into key
                                let row = key.Row
                                let col = key.Column
                                select _cells[row, col]).ToArray()).Select(source.AddAllDifferent))
                    {
                        yield return c;
                    }
                }

                return _constraints ?? (_constraints = GetAll().ToArray());
            }
        }

        protected override void OnSolved(EventArgs e)
        {
            var solver = Solver;

            ISudokuPuzzle solution = new SudokuPuzzle();

            for (var row = MinimumValue; row < MaximumValue; row++)
            {
                for (var col = MinimumValue; col < MaximumValue; col++)
                {
                    // TODO: TBD: I find it a little odd that you need to turn around and ask the Solver for the Value (or BooleanValue) of the IntVar, etc.
                    solution[row, col] = (int) solver.Value(_cells[row, col]);
                }
            }

            /* If we're here processing variables, it should be because we are processing the next
             * solution. However, in the event we still do not have a solution, then simply return. */

            if (!solution.IsSolved)
            {
                return;
            }

            // Make sure any valid Solutions we may have are set prior to invoking the Base.
            Solution = solution;

            base.OnSolved(e);
        }
    }
}
