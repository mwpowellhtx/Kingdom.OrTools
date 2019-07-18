namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class LongParameter : Parameter<long>
    {
        public LongParameter() : this(default)
        {
        }

        public LongParameter(long value) : base(value, InternalOrdinal)
        {
        }
    }
}
