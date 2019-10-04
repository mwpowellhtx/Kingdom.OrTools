namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_double)]
    public class AnnotatedDoubleParameter : DoubleParameter
    {
        public AnnotatedDoubleParameter()
        {
        }

        public AnnotatedDoubleParameter(double value) : base(value)
        {
        }
    }
}
