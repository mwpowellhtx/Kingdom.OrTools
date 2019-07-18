using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class MonthRepeatedParameter : RepeatedParameter<Month>
    {
        public MonthRepeatedParameter() : this(default)
        {
        }

        public MonthRepeatedParameter(Month value, params Month[] others) : base(new []{value}.Concat(others), InternalOrdinal)
        {
        }
    }
}
