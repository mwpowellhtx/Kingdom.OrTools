namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_boolean)]
    public class AnnotatedBooleanParameter : BooleanParameter
    {
        public AnnotatedBooleanParameter()
        {
        }

        public AnnotatedBooleanParameter(bool value) : base(value)
        {
        }
    }
}
