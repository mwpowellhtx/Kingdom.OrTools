namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class DoubleParameter : Parameter<double>
    {
        public DoubleParameter() : this(default)
        {
        }

        public DoubleParameter(double value) : base(value, InternalOrdinal)
        {
        }
    }
}
