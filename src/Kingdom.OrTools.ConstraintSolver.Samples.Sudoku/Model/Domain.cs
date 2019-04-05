using System;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    internal static class Domain
    {
        /// <summary>
        /// 0
        /// </summary>
        public const int MinimumValue = 0;

        /// <summary>
        /// 9
        /// </summary>
        public const int MaximumValue = 9;

        /// <summary>
        /// Gets the Square Root of <see cref="MaximumValue"/>.
        /// </summary>
        public static int GroupMaximumValue => (int) Math.Sqrt(MaximumValue);
    }
}
