using System;

namespace Kingdom.OrTools.Sat.Parameters
{
    // TODO: TBD: may rename to ParameterField instead of ParameterMember, but will run with this for the time being...
    [AttributeUsage(AttributeTargets.Field)]
    public class ParameterMemberNameAttribute : Attribute
    {
        public string MemberName { get; }

        public ParameterMemberNameAttribute(string memberName) => MemberName = memberName;
    }
}
