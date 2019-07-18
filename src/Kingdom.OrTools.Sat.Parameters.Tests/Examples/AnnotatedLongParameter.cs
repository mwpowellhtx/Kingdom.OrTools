namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    [ParameterName(annotated_long)]
    public class AnnotatedLongParameter : LongParameter
    {
        public AnnotatedLongParameter()
        {
        }

        public AnnotatedLongParameter(long value) : base(value)
        {
        }
    }
}
