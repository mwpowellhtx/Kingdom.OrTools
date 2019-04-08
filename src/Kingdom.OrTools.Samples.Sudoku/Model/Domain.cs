namespace Kingdom.OrTools.Samples.Sudoku
{
    internal static class Domain
    {
        /// <summary>
        /// 0
        /// </summary>
        public const int MinimumValue = 0;

        /// <summary>
        /// 3
        /// </summary>
        /// <see cref="MaximumValue"/>
        internal const int BlockSize = 3;

        /// <summary>
        /// <see cref="BlockSize"/> times itself.
        /// </summary>
        /// <see cref="BlockSize"/>
        public const int MaximumValue = BlockSize * BlockSize;
    }
}
