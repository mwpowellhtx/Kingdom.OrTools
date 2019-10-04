using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class IntegerRepeatedParameter : RepeatedParameter<int>
    {
        public IntegerRepeatedParameter() : this(default)
        {
        }

        public IntegerRepeatedParameter(int value, params int[] others) : base(new[] {value}.Concat(others), InternalOrdinal)
        {
        }
    }
}
