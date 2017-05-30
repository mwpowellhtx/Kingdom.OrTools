using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using Google.OrTools.ConstraintSolver;

    public class SudokuProblemSolverAspect : ProblemSolverAspectBase<Solver, IntVar, Constraint, SudokuProblemSolverAspect>
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

        /// <summary>
        /// Min: 1L
        /// </summary>
        public const long Min = 1L;

        /// <summary>
        /// Max: 9L
        /// </summary>
        public const long Max = 9L;

        /// <summary>
        /// Size: 9
        /// </summary>
        public const int Size = 9;

        /// <summary>
        /// Div: 3
        /// </summary>
        public const int Div = 3;

        public override IEnumerable<IntVar> GetVariables(Solver solver)
        {
            var s = solver;

            Cells = new IntVar[Size, Size];

            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    Cells[i, j] = s.MakeIntVar(Min, Max, $@"[{i},{j}]");
                }
            }

            foreach (var c in Cells.Flatten())
            {
                yield return c;
            }
        }

        private IEnumerable<IntVar> GetGroup(IEnumerable<int> rows, IEnumerable<int> columns)
        {
            return from i in rows from j in columns select Cells[i, j];
        }

        public override IEnumerable<Constraint> GetConstraints(Solver solver)
        {
            var s = solver;

            foreach (var cell in Cells.Flatten())
            {
                var c = s.MakeBetweenCt(cell, Min, Max);
                s.Add(c);
                yield return c;
            }

            for (var i = 0; i < Size; i++)
            {
                var perpIdx = Enumerable.Range(0, Size).ToArray();
                var rowOrColumnIdx = new[] {i};

                {
                    var vect = new IntVarVector(GetGroup(rowOrColumnIdx, perpIdx).ToList()).TrackClrObject(this);
                    var c = s.MakeAllDifferent(vect).TrackClrObject(this);
                    s.Add(c);
                    yield return c;
                }

                {
                    var vect = new IntVarVector(GetGroup(perpIdx, rowOrColumnIdx).ToList()).TrackClrObject(this);
                    var c = s.MakeAllDifferent(vect).TrackClrObject(this);
                    s.Add(c);
                    yield return c;
                }
            }

            for (var i = 0; i < Div; i++)
            {
                var rowIdx = Enumerable.Range(i * Div, Div).ToArray();

                for (var j = 0; j < Div; j++)
                {
                    var columnIdx = Enumerable.Range(j * Div, Div).ToArray();
                    var vect = new IntVarVector(GetGroup(rowIdx, columnIdx).ToList()).TrackClrObject(this);
                    var c = s.MakeAllDifferent(vect).TrackClrObject(this);
                    s.Add(c);
                    yield return c;
                }
            }
        }

        /* In this case we do not care about any overlapping concerns. We just want to demonstrate
         * the fundamental modeling, constraints, etc. */
    }
}
