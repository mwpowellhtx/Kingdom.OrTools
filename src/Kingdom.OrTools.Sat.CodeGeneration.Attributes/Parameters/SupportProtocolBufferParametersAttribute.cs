using System;
using System.Diagnostics;

namespace Kingdom.OrTools.Sat.CodeGeneration.Attributes
{
    using Code.Generation.Roslyn;

    /// <summary>
    /// The Code Generator must be named CpSatParametersGenerator in the
    /// Kingdom.OrTools.Sat.CodeGeneration.Generators namespace furnished from a separate
    /// assembly.
    /// </summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Assembly)
     , CodeGenerationAttribute("Kingdom.OrTools.Sat.CodeGeneration.Generators.CpSatParametersAssemblyCodeGenerator, Kingdom.OrTools.Sat.CodeGeneration")
     , Conditional("CodeGeneration")]
    public class SupportProtocolBufferParametersAttribute : Attribute
    {
    }
}
