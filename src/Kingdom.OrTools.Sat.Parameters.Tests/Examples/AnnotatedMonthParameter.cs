namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_month)]
    public class AnnotatedMonthParameter : MonthParameter
    {
        public AnnotatedMonthParameter()
        {
        }

        public AnnotatedMonthParameter(Month value) : base(value)
        {
        }
    }
}
