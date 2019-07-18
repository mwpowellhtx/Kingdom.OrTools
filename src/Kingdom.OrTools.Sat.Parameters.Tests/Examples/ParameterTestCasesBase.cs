namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    // TODO: TBD: fill in some gaps here when we determine what the shape and composition of the parameter unit test should be...
    public class IntegerParameter : Parameter<int>
    {
        public IntegerParameter() : this(default)
        {
        }

        public IntegerParameter(int value) : base(value, InternalOrdinal)
        {
        }
    }
}
