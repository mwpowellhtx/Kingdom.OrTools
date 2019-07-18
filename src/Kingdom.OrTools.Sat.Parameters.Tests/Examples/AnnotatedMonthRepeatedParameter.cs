namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_month_repeated)]
    public class AnnotatedMonthRepeatedParameter : MonthRepeatedParameter
    {
        public AnnotatedMonthRepeatedParameter()
        {
        }

        public AnnotatedMonthRepeatedParameter(Month value, params Month[] others) : base(value, others)
        {
        }
    }
}
