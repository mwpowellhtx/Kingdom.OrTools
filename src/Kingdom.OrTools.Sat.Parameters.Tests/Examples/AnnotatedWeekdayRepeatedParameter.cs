namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_weekday_repeated)]
    public class AnnotatedWeekdayRepeatedParameter : WeekdayRepeatedParameter
    {
        public AnnotatedWeekdayRepeatedParameter()
        {
        }

        public AnnotatedWeekdayRepeatedParameter(AnnotatedWeekday value, params AnnotatedWeekday[] others) : base(value, others)
        {
        }
    }
}
