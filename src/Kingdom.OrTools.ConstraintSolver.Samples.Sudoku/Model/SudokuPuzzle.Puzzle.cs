using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using static Domain;
    using static String;

    public partial class SudokuPuzzle : ISudokuPuzzle
    {
        /// <inheritdoc />
        public bool IsSolved
        {
            get
            {
                var rows = Rows;
                var columns = Columns;
                var blocks = Blocks;
                var concatenated = rows.Concat(columns).Concat(blocks).ToArray();
                return concatenated.All(x => x.Distinct().Count() == MaximumValue)
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
            for (var major = MinimumValue; major < MaximumValue; major++)
            {
                var result = new Dictionary<Address, int>();
                for (var minor = MinimumValue; minor < MaximumValue; minor++)
                {
                    var address = addressable(major, minor);
                    result.Add(address, this[address]);
                }

                yield return result;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IDictionary<Address, int>> Rows => GetLines(
            (major, minor) => new Address(major, minor)
        );

        /// <inheritdoc />
        public IEnumerable<IDictionary<Address, int>> Columns => GetLines(
            (major, minor) => new Address(minor, major)
        );

        /// <summary>
        /// Returns the block at each <paramref name="blockRow"/> and <paramref name="blockColumn"/>.
        /// </summary>
        /// <param name="blockRow"></param>
        /// <param name="blockColumn"></param>
        /// <returns></returns>
        private IDictionary<Address, int> GetBlock(int blockRow, int blockColumn)
        {
            //// TODO: TBD: do range checking on the values and throw...
            //Debug.Assert(blockRow >= 0 && blockRow < 3);
            //Debug.Assert(blockColumn >= 0 && blockColumn < 3);

            var result = new Dictionary<Address, int>();

            var maximumValue = GroupMaximumValue;

            for (var row = MinimumValue; row < maximumValue; row++)
            {
                for (var column = MinimumValue; column < maximumValue; column++)
                {
                    var address = new Address(blockRow * maximumValue + row, blockColumn * maximumValue + column);
                    result[address] = this[address];
                }
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<IDictionary<Address, int>> Blocks
        {
            get
            {
                var maximumValue = GroupMaximumValue;

                for (var blockRow = MinimumValue; blockRow < maximumValue; blockRow++)
                {
                    for (var blockColumn = MinimumValue; blockColumn < maximumValue; blockColumn++)
                    {
                        yield return GetBlock(blockRow, blockColumn);
                    }
                }
            }
        }

        /// <inheritdoc />
        int[,] ISudokuPuzzle.Values
        {
            get
            {
                var result = new int[MaximumValue, MaximumValue];
                foreach (var item in this)
                {
                    result[item.Key.Row, item.Key.Column] = item.Value;
                }

                return result;
            }
        }

        /// <inheritdoc />
        int ISudokuPuzzle.this[Address key]
        {
            get => _grid[key];
            set => _grid[key] = value;
        }

        /// <inheritdoc />
        int ISudokuPuzzle.this[int row, int column]
        {
            get => _grid[new Address(row, column)];
            set => _grid[new Address(row, column)] = value;
        }

        /// <inheritdoc />
        void ISudokuPuzzle.PrettyPrint(PrettyPrintCallback callback)
        {
            string Render()
            {
                var sb = new StringBuilder();

                var solved = IsSolved;

                string GetSolvedPrefix() => solved ? @"" : @"Not ";
                char GetSolvedSuffix() => solved ? '!' : '.';

                sb.AppendLine($"{GetSolvedPrefix()}Solved{GetSolvedSuffix()}");

                var rows = Rows.Select(x => x.Values.ToArray());

                foreach (var row in rows)
                {
                    sb.AppendLine("\t" + Join(@" ", row.Select(x => x.ToString())));
                }

                return $"{sb}";
            }

            callback?.Invoke(Render());
        }
    }
}
