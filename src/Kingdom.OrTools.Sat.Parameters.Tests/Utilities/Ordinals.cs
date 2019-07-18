namespace Kingdom.OrTools.Sat.Parameters
{
    internal static class Ordinals
    {
        private static long _ordinal;

        /// <summary>
        /// Gets the Next InternalOrdinal in sequence.
        /// </summary>
        /// <remarks>This is unused except to inform the <see cref="IParameter.Ordinal"/> values.
        /// Otherwise, this is basically an unused part of the API framework scaffold. It is more
        /// an artifact of the Protocol Buffer Message Items than anything else.</remarks>
        internal static long InternalOrdinal => ++_ordinal;

        /// <summary>
        /// Gets the CurrentOrdinal value.
        /// </summary>
        /// <see cref="InternalOrdinal"/>
        internal static long CurrentOrdinal => _ordinal;
    }
}
