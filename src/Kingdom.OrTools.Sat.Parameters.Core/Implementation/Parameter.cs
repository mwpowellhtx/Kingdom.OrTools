namespace Kingdom.OrTools.Sat.Parameters
{
    /// <inheritdoc />
    public abstract class Parameter : IParameter
    {
        /// <inheritdoc />
        public long Ordinal { get; }

        // TODO: TBD: parameters have a canonical name? based on the parsed bits...
        // TODO: TBD: along same lines as ctor-given ordinal, pass canonical name via ctor...
        // TODO: TBD: might also call it lexical, or lexicon, based on parsed option names, etc

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="ordinal"></param>
        protected Parameter(long ordinal)
        {
            Ordinal = ordinal;
        }
    }
}
