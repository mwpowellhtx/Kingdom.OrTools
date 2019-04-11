using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using Google.OrTools.ConstraintSolver;
    using static Kingdom.OrTools.Samples.Sudoku.Domain;
    using static Kingdom.OrTools.Samples.Sudoku.SudokuPuzzle;

    public class SudokuProblemSolverAspect : ProblemSolverAspectBase<Solver, Solver, IntVar, Constraint, SudokuProblemSolverAspect>
    {
        /// <summary>
        /// Internal Constructor
        /// </summary>
        internal SudokuProblemSolverAspect()
        {
        }

        /// <summary>
        /// Gets the Cells.
        /// </summary>
        public IntVar[,] Cells { get; private set; }

        public override IEnumerable<IntVar> GetVariables(Solver source)
        {
            Cells = new IntVar[Size, Size];

            for (var row = MinimumValue; row < MaximumValue; row++)
            {
                for (var col = MinimumValue; col < MaximumValue; col++)
                {
                    Cells[row, col] = source.MakeIntVar(MinimumValue + 1, MaximumValue, $"[{row},{col}]");
                }
            }

            foreach (var c in Cells.Flatten())
            {
                yield return c;
            }
        }

        private IEnumerable<IntVar> GetGroup(IEnumerable<int> rows, IEnumerable<int> columns)
            => from row in rows from col in columns select Cells[row, col];

        public override IEnumerable<Constraint> GetConstraints(Solver source)
        {
            for (var i = MinimumValue; i < MaximumValue; i++)
            {
                var perpendicularIndexes = Enumerable.Range(0, Size).ToArray();
                var rowOrColumnIndexes = new[] {i};

                {
                    var vector = new IntVarVector(GetGroup(rowOrColumnIndexes, perpendicularIndexes).ToList()).TrackClrObject(this);
                    var c = source.MakeAllDifferent(vector).TrackClrObject(this);
                    source.Add(c);
                    yield return c;
                }

                {
                    var vector = new IntVarVector(GetGroup(perpendicularIndexes, rowOrColumnIndexes).ToList()).TrackClrObject(this);
                    var c = source.MakeAllDifferent(vector).TrackClrObject(this);
                    source.Add(c);
                    yield return c;
                }
            }

            for (var i = MinimumValue; i < BlockSize; i++)
            {
                var rowIndexes = Enumerable.Range(i * BlockSize, BlockSize).ToArray();

                for (var j = MinimumValue; j < BlockSize; j++)
                {
                    var columnIndexes = Enumerable.Range(j * BlockSize, BlockSize).ToArray();
                    var vector = new IntVarVector(GetGroup(rowIndexes, columnIndexes).ToList()).TrackClrObject(this);
                    var c = source.MakeAllDifferent(vector).TrackClrObject(this);
                    source.Add(c);
                    yield return c;
                }
            }
        }

        /* In this case we do not care about any overlapping concerns. We just want to demonstrate
         * the fundamental modeling, constraints, etc. */
    }
}
