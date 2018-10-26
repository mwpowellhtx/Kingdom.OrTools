using System;
using System.IO;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    using Xunit.Abstractions;

    /// <summary>
    /// 
    /// </summary>
    public static class SudokuExtensionMethods
    {
        private const int Min = 0;
        private const int Max = 9;

        /// <summary>
        /// Verifies the <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        public static void VerifyValue(this int value)
        {
            if (value.TryVerifyValue())
            {
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value)
                , $"{nameof(value)}({value}) must be between {Min} and {Max}"
            );
        }

        /// <summary>
        /// Tries to verify the <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryVerifyValue(this int value) => value >= Min && value <= Max;

        /// <summary>
        /// Tries the Solved <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySolvedValue(this int value) => value > Min && value <= Max;

        /// <summary>
        /// Returns a Sudoku SudokuPuzzle based on the <paramref name="values"/>. Values are
        /// treated in row major manner. Each value is validated first and foremost.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static SudokuPuzzle ToSudokuPuzzle(this int[] values)
        {
            foreach (var value in values)
            {
                value.VerifyValue();
            }

            var result = new SudokuPuzzle();

            for (var row = Min; row < Max; row++)
            {
                for (var column = Min; column < Max; column++)
                {
                    result[new Address(row, column)] = values[row * Max + column];
                }
            }

            return result;
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Included for legacy purposes. Was using this, but with adoption of xUnit.net for unit
        /// testing purposes, instead using the <see cref="ITestOutputHelper"/> along these lines.
        /// </summary>
        /// <param name="puzzle"></param>
        /// <param name="writer"></param>
        public static void PrettyPrint(this ISudokuPuzzle puzzle, TextWriter writer)
            => puzzle?.PrettyPrint(s =>
            {
                writer.Write(s);
                writer.Flush();
            });
    }
}
