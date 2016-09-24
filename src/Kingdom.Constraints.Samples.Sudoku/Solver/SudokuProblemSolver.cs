using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Constraints.Samples.Sudoku
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:http://github.com/google/or-tools/blob/master/examples/python/sudoku.py"/>
    public class SudokuProblemSolver : OrProblemSolverBase<SudokuProblemSolver>
    {
        /// <summary>
        /// SudokuPuzzle backing field.
        /// </summary>
        private readonly ISudokuPuzzle _puzzle;

        /// <summary>
        /// Solution backing field.
        /// </summary>
        private ISudokuPuzzle _solution = new SudokuPuzzle();

        /// <summary>
        /// Gets the Solution.
        /// </summary>
        public ISudokuPuzzle Solution
        {
            get { return _solution; }
            private set { _solution = value; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="aPuzzle"></param>
        public SudokuProblemSolver(ISudokuPuzzle aPuzzle)
            : base(@"Sudoku Solver")
        {
            _puzzle = aPuzzle;
        }

        private int[,] _matrix;

        /// <summary>
        /// Returns a <see cref="Solver"/> seed.
        /// </summary>
        /// <returns></returns>
        protected override int GetSolverSeed()
        {
            return new Random().Next();
        }

        /// <summary>
        /// Initializes the <param name="solver"></param>.
        /// </summary>
        /// <param name="solver"></param>
        protected override void Initialize(Solver solver)
        {
            base.Initialize(solver);

            _matrix = _puzzle.Values;
        }

        /// <summary>
        /// Returns a made cell variable.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public IntVar MakeCell(Solver solver, int row, int column)
        {
            var result = solver.MakeIntVar(0, 9, string.Format(@"SudokuPuzzle[{0}, {1}]", row, column));
            ClrCreatedObjects.Add(result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly IntVar[,] _cells = new IntVar[9, 9];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        protected override void PrepareVariables(Solver solver)
        {
            foreach (var cell in (SudokuPuzzle) _puzzle)
            {
                var row = cell.Key.Row;
                var column = cell.Key.Column;
                _cells[row, column] = MakeCell(solver, row, column);
            }
        }

        /// <summary>
        /// Makes an initial constraint on each cell if possible.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryInitialMakeConstraint(Solver solver, IntExpr variable, int value)
        {
            var basic = solver.MakeBetweenCt(variable, 1, 9);
            solver.Add(basic);
            ClrCreatedObjects.Add(basic);

            if (!value.TrySolvedValue()) return false;

            var initialized = variable == value;
            solver.Add(initialized);
            ClrCreatedObjects.Add(initialized);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        private bool MakeAllDifferentConstraints(Solver solver, IEnumerable<IDictionary<Address, int>> groups)
        {
            var groupedCells = groups.Select(g => g.Select(
                h => _cells[h.Key.Row, h.Key.Column]).ToList()).ToArray();

            foreach (var different in groupedCells
                .Select(g => solver.MakeAllDifferent(new IntVarVector(g))))
            {
                solver.Add(different);
                ClrCreatedObjects.Add(different);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        protected override void PrepareConstraints(Solver solver)
        {
            foreach (var cell in (SudokuPuzzle) _puzzle)
            {
                var row = cell.Key.Row;
                var column = cell.Key.Column;

                TryInitialMakeConstraint(solver, _cells[row, column], _puzzle[row, column]);
            }

            MakeAllDifferentConstraints(solver, _puzzle.Rows.Concat(_puzzle.Columns).Concat(_puzzle.Blocks));
        }

        /// <summary>
        /// Vectored backing field.
        /// </summary>
        private IntVarVector _vectored;

        /// <summary>
        /// Vector the variables in row major manner.
        /// </summary>
        private IntVarVector Vectored
        {
            get
            {
                if (_vectored == null)
                {
                    var variables = new List<IntVar>();

                    for (var row = 0; row < 9; row++)
                        for (var column = 0; column < 9; column++)
                            variables.Add(_cells[row, column]);

                    _vectored = new IntVarVector(variables);
                }
                return _vectored;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="collector"></param>
        protected override void PrepareSolutionCollector(Solver solver, SolutionCollector collector)
        {
            collector.Add(Vectored);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override bool TryMakeDecisionBuilder(Solver solver, out DecisionBuilder builder)
        {
            // Want the fewest degrees of freedom first.
            var variableChooser = new VariableChooser(x => 0);

            // Rule out (and/or keep in the running) only those choices that are still remaining.
            var valueChooser = new ValueChooser((x, i) => 0);

            //TODO: may need/want custom choosers to help the issue along...
            builder = solver.MakePhase(Vectored, Solver.INT_VAR_SIMPLE, Solver.INT_VALUE_SIMPLE);

            ClrCreatedObjects.Add(builder);

            return builder != null;
        }

        /// <summary>
        /// Solved event.
        /// </summary>
        public event EventHandler<EventArgs> Solved;

        /// <summary>
        /// Raises the <see cref="Solved"/> event.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseSolved(EventArgs e)
        {
            if (Solved == null) return;
            Solved(this, e);
        }

        /// <summary>
        /// Tries to receive the next available <paramref name="assignment"/>.
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected override bool TryReceiveAssignment(Assignment assignment)
        {
            var candidate = new SudokuPuzzle();
            ISudokuPuzzle local = candidate;

            for (var row = 0; row < 9; row++)
            {
                for (var column = 0; column < 9; column++)
                {
                    local[row, column] = (int) assignment.Value(_cells[row, column]);
                }
            }

            if (local.IsSolved)
            {
                Solution = local;
                RaiseSolved(EventArgs.Empty);
            }

            return local.IsSolved;
        }
    }
}
