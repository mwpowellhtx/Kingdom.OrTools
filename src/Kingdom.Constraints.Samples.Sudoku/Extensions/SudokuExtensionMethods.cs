using System;

namespace Kingdom.Constraints.Samples.Sudoku
{
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
            if (value.TryVerifyValue()) return;

            throw new ArgumentOutOfRangeException(@"value",
                string.Format(@"value must be between {0} and {1}", 0, 9));
        }

        /// <summary>
        /// Tries to verify the value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryVerifyValue(this int value)
        {
            return value >= 0 && value <= 9;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySolvedValue(this int value)
        {
            return value > 0 && value <= 9;
        }

        /// <summary>
        /// Returns a Sudoku SudokuPuzzle based on the <paramref name="values"/>. Values are
        /// treated in row major manner. Each value is validated first and foremost.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static SudokuPuzzle ToSudokuPuzzle(this int[] values)
        {
            foreach (var value in values)
                value.VerifyValue();

            var result = new SudokuPuzzle();

            for (var row = 0; row < 9; row++)
                for (var column = 0; column < 9; column++)
                    result[new Address(row, column)] = values[row*9 + column];

            return result;
        }
    }
}
