namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class MonthParameter : Parameter<Month>
    {
        public MonthParameter() : this(default)
        {
        }

        public MonthParameter(Month value) : base(value, InternalOrdinal)
        {
        }
    }
}
