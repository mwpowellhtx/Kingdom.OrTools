using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters.Examples
{
    using static Ordinals;

    public class DoubleRepeatedParameter : RepeatedParameter<double>
    {
        public DoubleRepeatedParameter() : this(default)
        {
        }

        public DoubleRepeatedParameter(double value, params double[] others) : base(new[] {value}.Concat(others), InternalOrdinal)
        {
        }
    }
}
