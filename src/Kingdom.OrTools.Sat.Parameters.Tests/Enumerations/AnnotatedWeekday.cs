namespace Kingdom.OrTools.Sat.Parameters
{
    using static Names;

    public enum AnnotatedWeekday : long
    {
        [ParameterMemberName(MONDAY)]
        Monday = 0L,

        [ParameterMemberName(TUESDAY)]
        Tuesday,

        [ParameterMemberName(WEDNESDAY)]
        Wednesday,

        [ParameterMemberName(THURSDAY)]
        Thursday,

        [ParameterMemberName(FRIDAY)]
        Friday
    }
}
