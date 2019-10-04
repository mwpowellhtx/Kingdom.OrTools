namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class BooleanParameter : Parameter<bool>
    {
        public BooleanParameter() : this(default)
        {
        }

        public BooleanParameter(bool value) : base(value, InternalOrdinal)
        {
        }
    }
}
