using System;
using System.Diagnostics;

namespace Kingdom.OrTools.Sat
{
    using Code.Generation.Roslyn;

    /// <summary>
    /// The Code Generator must be named CpSatParametersGenerator in the
    /// Kingdom.OrTools.Sat namespace furnished from a separate assembly.
    /// </summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Assembly)
     , CodeGenerationAttribute("Kingdom.OrTools.Sat.CpSatParametersGenerator")
     , Conditional("CodeGeneration")]
    public class SupportProtocolBufferParametersAttribute : Attribute
    {
    }
}
