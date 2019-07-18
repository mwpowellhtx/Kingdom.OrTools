using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters.Examples
{
    using static Ordinals;

    public class LongRepeatedParameter : RepeatedParameter<long>
    {
        public LongRepeatedParameter() : this(default)
        {
        }

        public LongRepeatedParameter(long value, params long[] others) : base(new[] {value}.Concat(others), InternalOrdinal)
        {
        }
    }
}
