using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class WeekdayRepeatedParameter : RepeatedParameter<AnnotatedWeekday>
    {
        public WeekdayRepeatedParameter() : this(default)
        {
        }

        public WeekdayRepeatedParameter(AnnotatedWeekday value, params AnnotatedWeekday[] others) : base(new[] {value}.Concat(others), InternalOrdinal)
        {
        }
    }
}
