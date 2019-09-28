namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Code.Generation.Roslyn;

    /// <inheritdoc />
    internal class CodeGenerationServiceManager : ServiceManager<GeneratedSyntaxTreeDescriptor
        , OrToolsSatGeneratedSyntaxTreeRegistry, OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter>
    {
        /// <summary>
        /// Internal Constructor.
        /// </summary>
        /// <inheritdoc />
        /// <remarks>Leverages Implicit type conversion Operators implemented
        /// by the Internal Registry and Data Transfer Object classes.</remarks>
        internal CodeGenerationServiceManager(string outputDirectory, string registryFileName)
            : base(outputDirectory, registryFileName
                , () => OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter.Converter)
        {
        }
    }
}
