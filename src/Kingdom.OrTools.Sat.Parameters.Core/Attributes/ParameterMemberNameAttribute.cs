using System;

namespace Kingdom.OrTools.Sat.Parameters
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class ParameterMemberNameAttribute : Attribute
    {
        public string MemberName { get; }

        public ParameterMemberNameAttribute(string memberName) => MemberName = memberName;
    }
}
