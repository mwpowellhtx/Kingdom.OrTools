using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters.Examples
{
    using static Ordinals;

    public class BooleanRepeatedParameter : RepeatedParameter<bool>
    {
        public BooleanRepeatedParameter() : this(default)
        {
        }

        public BooleanRepeatedParameter(bool value, params bool[] others) : base(new[] {value}.Concat(others), InternalOrdinal)
        {
        }
    }
}
