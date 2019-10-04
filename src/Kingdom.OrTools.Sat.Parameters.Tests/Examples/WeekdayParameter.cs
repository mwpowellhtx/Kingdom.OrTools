namespace Kingdom.OrTools.Sat.Parameters
{
    using static Ordinals;

    public class WeekdayParameter : Parameter<AnnotatedWeekday>
    {
        public WeekdayParameter() : this(default)
        {
        }

        public WeekdayParameter(AnnotatedWeekday value) : base(value, InternalOrdinal)
        {
        }
    }
}
