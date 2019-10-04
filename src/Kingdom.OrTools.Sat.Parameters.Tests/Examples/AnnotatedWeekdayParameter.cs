namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_weekday)]
    public class AnnotatedWeekdayParameter : WeekdayParameter
    {
        public AnnotatedWeekdayParameter()
        {
        }

        public AnnotatedWeekdayParameter(AnnotatedWeekday value) : base(value)
        {
        }
    }
}
