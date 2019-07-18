namespace Kingdom.OrTools.Sat.Parameters.Examples
{
    using static Names;

    [ParameterName(annotated_long_repeated)]
    public class AnnotatedLongRepeatedParameter : LongRepeatedParameter
    {
        public AnnotatedLongRepeatedParameter()
        {
        }

        public AnnotatedLongRepeatedParameter(long value, params long[] others) : base(value, others)
        {
        }
    }
}
