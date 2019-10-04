namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_boolean_repeated)]
    public class AnnotatedBooleanRepeatedParameter : BooleanRepeatedParameter
    {
        public AnnotatedBooleanRepeatedParameter()
        {
        }

        public AnnotatedBooleanRepeatedParameter(bool value, params bool[] others) : base(value, others)
        {
        }
    }
}
