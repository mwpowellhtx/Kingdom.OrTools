namespace Kingdom.OrTools.Sat.Parameters.Examples
{
    using static Names;

    [ParameterName(annotated_double_repeated)]
    public class AnnotatedDoubleRepeatedParameter : DoubleRepeatedParameter
    {
        public AnnotatedDoubleRepeatedParameter()
        {
        }

        public AnnotatedDoubleRepeatedParameter(double value, params double[] others) : base(value, others)
        {
        }
    }
}
