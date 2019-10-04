namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_integer)]
    public class AnnotatedIntegerParameter : IntegerParameter
    {
        public AnnotatedIntegerParameter()
        {
        }

        public AnnotatedIntegerParameter(int value) : base(value)
        {
        }
    }
}
