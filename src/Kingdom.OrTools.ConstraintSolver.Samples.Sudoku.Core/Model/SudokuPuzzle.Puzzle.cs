using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using static String;

    public partial class SudokuPuzzle : ISudokuPuzzle
    {
        /// <summary>
        /// 0
        /// </summary>
        /// <see cref="MinSize"/>
        internal const int MinBlockSize = 0;

        /// <summary>
        /// 3
        /// </summary>
        /// <see cref="Size"/>
        internal const int BlockSize = 3;

        /// <summary>
        /// <see cref="MinBlockSize"/> squared.
        /// </summary>
        /// <see cref="MinBlockSize"/>
        internal const int MinSize = MinBlockSize * MinBlockSize;

        /// <summary>
        /// <see cref="BlockSize"/> squared.
        /// </summary>
        /// <see cref="BlockSize"/>
        internal const int Size = BlockSize * BlockSize;

        /// <summary>
        /// Returns whether IsSolved.
        /// </summary>
        /// <inheritdoc />
        public bool IsSolved
        {
            get
            {
                var rows = Rows;
                var columns = Columns;
                var blocks = Blocks;
                var concatenated = rows.Concat(columns).Concat(blocks).ToArray();
                return concatenated.All(x => x.Distinct().Count() == Size)
                       && concatenated.SelectMany(x => x.Values).All(x => x.TrySolvedValue());
            }
        }

        /// <summary>
        /// Returns the lines given how to <paramref name="addressable"/> maps the major
        /// and minor row and column components.
        /// </summary>
        /// <param name="addressable"></param>
        /// <returns></returns>
        public IEnumerable<IDictionary<Address, int>> GetLines(Func<int, int, Address> addressable)
        {
            for (var major = 0; major < Size; major++)
            {
                var result = new Dictionary<Address, int>();
                for (var minor = 0; minor < Size; minor++)
                {
                    var address = addressable(major, minor);
                    result.Add(address, this[address]);
                }

                yield return result;
            }
        }

        /// <summary>
        /// Gets the Rows.
        /// </summary>
        /// <inheritdoc />
        public IEnumerable<IDictionary<Address, int>> Rows
            => GetLines((major, minor) => new Address(major, minor));

        /// <summary>
        /// Gets the Columns.
        /// </summary>
        /// <inheritdoc />
        public IEnumerable<IDictionary<Address, int>> Columns
            => GetLines((major, minor) => new Address(minor, major));

        /// <summary>
        /// Returns the Block at each <paramref name="row"/> and <paramref name="column"/>.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private IDictionary<Address, int> GetBlock(int row, int column)
        {
            Debug.Assert(row >= 0 && row < BlockSize);
            Debug.Assert(column >= 0 && column < BlockSize);

            var result = new Dictionary<Address, int>();

            for (var rowDelta = 0; rowDelta < BlockSize; rowDelta++)
            {
                for (var columnDelta = 0; columnDelta < BlockSize; columnDelta++)
                {
                    var address = new Address(row * BlockSize + rowDelta, column * BlockSize + columnDelta);
                    result[address] = this[address];
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the Blocks.
        /// </summary>
        /// <inheritdoc />
        public IEnumerable<IDictionary<Address, int>> Blocks
        {
            get
            {
                for (var row = 0; row < BlockSize; row++)
                {
                    for (var column = 0; column < BlockSize; column++)
                    {
                        yield return GetBlock(row, column);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Matrix Values.
        /// </summary>
        /// <inheritdoc />
        int[,] ISudokuPuzzle.Values
        {
            get
            {
                var result = new int[Size, Size];
                foreach (var item in this)
                {
                    result[item.Key.Row, item.Key.Column] = item.Value;
                }

                return result;
            }
        }

        /// <summary>
        /// Get or Set Indexer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <inheritdoc />
        int ISudokuPuzzle.this[Address key]
        {
            get => _grid[key];
            set => _grid[key] = value;
        }

        /// <summary>
        /// Get or Set Indexer.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <inheritdoc />
        int ISudokuPuzzle.this[int row, int column]
        {
            get => _grid[new Address(row, column)];
            set => _grid[new Address(row, column)] = value;
        }

        void ISudokuPuzzle.PrettyPrint(PrettyPrintCallback callback)
        {
            var solved = IsSolved;

            const string empty = "";
            const string not = "Not ";

            var sb = new StringBuilder();

            sb.AppendLine($"{(solved ? empty : not)}Solved{(solved ? '!' : '.')}");

            var rows = Rows.Select(x => x.Values.ToArray());

            foreach (var row in rows)
            {
                sb.AppendLine($"\t{Join(@" ", row.Select(x => x.ToString()))}");
            }

            callback($"{sb}");
        }
    }
}
