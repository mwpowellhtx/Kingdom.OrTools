using System;
//using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Code.Generation.Roslyn;

    /// <inheritdoc cref="IOrToolsSatPurgingRegistrySet{T}"/>
    internal class OrToolsSatGeneratedSyntaxTreeRegistry
        : GeneratedSyntaxTreeRegistry
            , IOrToolsSatPurgingRegistrySet<GeneratedSyntaxTreeDescriptor>
    {
        /// <inheritdoc />
        public Version GoogleOrToolsVersion { get; set; }

        /// <inheritdoc />
        public OrToolsSatGeneratedSyntaxTreeRegistry()
        {
            /* Yes, the default should be this. Which, when we deserialize, if the version
             * is detected to have updated, then we should also re-generate code. */

            GoogleOrToolsVersion = SatParameterCodeGeneratorService.GoogleOrToolsVersion;
        }

        ///// <summary>
        ///// Returns the Implicitly converted <paramref name="registry"/> as an
        ///// <see cref="OrToolsSatGeneratedSyntaxTreeRegistryTransferObject"/>.
        ///// </summary>
        ///// <param name="registry"></param>
        //public static implicit operator OrToolsSatGeneratedSyntaxTreeRegistryTransferObject(OrToolsSatGeneratedSyntaxTreeRegistry registry)
        //{
        //    const int fieldCount = 3;
        //    var dto = new OrToolsSatGeneratedSyntaxTreeRegistryTransferObject
        //    {
        //        GoogleOrToolsVersion = registry.GoogleOrToolsVersion.ToString(fieldCount)
        //    };
        //    registry.ToList().ForEach(dto.Descriptors.Add);
        //    return dto;
        //}
    }
}
