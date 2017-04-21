using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    public partial class SudokuPuzzle : ISudokuPuzzle
    {
        /// <summary>
        /// Returns whether IsSolved.
        /// </summary>
        public bool IsSolved
        {
            get
            {
                var rows = Rows;
                var columns = Columns;
                var blocks = Blocks;
                var concatenated = rows.Concat(columns).Concat(blocks).ToArray();
                return concatenated.All(x => x.Distinct().Count() == 9)
                       && concatenated.SelectMany(x => x.Values).All(x => x.TrySolvedValue());
            }
        }

        /// <summary>
        /// Returns the lines given how to <paramref name="addressable"/> maps
        /// the major and minor row and column components.
        /// </summary>
        /// <param name="addressable"></param>
        /// <returns></returns>
        public IEnumerable<IDictionary<Address, int>> GetLines(Func<int, int, Address> addressable)
        {
            for (var major = 0; major < 9; major++)
            {
                var result = new Dictionary<Address, int>();
                for (var minor = 0; minor < 9; minor++)
                {
                    var address = addressable(major, minor);
                    result.Add(address, this[address]);
                }
                yield return result;
            }
        }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        public IEnumerable<IDictionary<Address, int>> Rows
        {
            get { return GetLines((major, minor) => new Address(major, minor)); }
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        public IEnumerable<IDictionary<Address, int>> Columns
        {
            get { return GetLines((major, minor) => new Address(minor, major)); }
        }

        /// <summary>
        /// Returns the block at each <paramref name="blockRow"/> and <paramref name="blockColumn"/>.
        /// </summary>
        /// <param name="blockRow"></param>
        /// <param name="blockColumn"></param>
        /// <returns></returns>
        private IDictionary<Address, int> GetBlock(int blockRow, int blockColumn)
        {
            Debug.Assert(blockRow >= 0 && blockRow < 3);
            Debug.Assert(blockColumn >= 0 && blockColumn < 3);

            var result = new Dictionary<Address, int>();

            for (var row = 0; row < 3; row++)
            {
                for (var column = 0; column < 3; column++)
                {
                    var address = new Address(blockRow*3 + row, blockColumn*3 + column);
                    result[address] = this[address];
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the blocks.
        /// </summary>
        public IEnumerable<IDictionary<Address, int>> Blocks
        {
            get
            {
                for (var blockRow = 0; blockRow < 3; blockRow++)
                    for (var blockColumn = 0; blockColumn < 3; blockColumn++)
                        yield return GetBlock(blockRow, blockColumn);
            }
        }

        /// <summary>
        /// Gets the Matrix Values.
        /// </summary>
        int[,] ISudokuPuzzle.Values
        {
            get
            {
                var result = new int[9, 9];
                foreach (var item in this)
                    result[item.Key.Row, item.Key.Column] = item.Value;
                return result;
            }
        }

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int ISudokuPuzzle.this[Address key]
        {
            get { return _grid[key]; }
            set { _grid[key] = value; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        int ISudokuPuzzle.this[int row, int column]
        {
            get { return _grid[new Address(row, column)]; }
            set { _grid[new Address(row, column)] = value; }
        }

        /// <summary>
        /// Writes the puzzle to the writer.
        /// </summary>
        /// <param name="writer"></param>
        void ISudokuPuzzle.PrettyPrint(TextWriter writer)
        {
            var solved = IsSolved;

            writer.WriteLine(@"{0}Solved{1}", solved ? @"" : @"Not ", solved ? '!' : '.');

            var rows = Rows.Select(x => x.Values.ToArray());

            foreach (var row in rows)
                writer.WriteLine("\t" + string.Join(@" ", row.Select(x => x.ToString())));

            writer.Flush();
        }
    }
}
