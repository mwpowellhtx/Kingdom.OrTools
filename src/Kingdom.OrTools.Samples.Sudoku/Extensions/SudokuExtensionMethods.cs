using System;

namespace Kingdom.OrTools.Samples.Sudoku
{
    using static Domain;

    /// <summary>
    /// 
    /// </summary>
    public static class SudokuExtensionMethods
    {
        /// <summary>
        /// Verifies the value.
        /// </summary>
        /// <param name="value"></param>
        public static void VerifyValue(this int value)
        {
            if (value.TryVerifyValue())
            {
                return;
            }

            throw new ArgumentOutOfRangeException(nameof(value), $"value must be between {MinimumValue} and {MaximumValue}");
        }

        /// <summary>
        /// Tries to verify the value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryVerifyValue(this int value) => value >= MinimumValue && value <= MaximumValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySolvedValue(this int value) => value > MinimumValue && value <= MaximumValue;

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

            for (var row = MinimumValue; row < MaximumValue; row++)
            {
                for (var column = MinimumValue; column < MaximumValue; column++)
                {
                    result[new Address(row, column)] = values[row * MaximumValue + column];
                }
            }

            return result;
        }
    }
}
