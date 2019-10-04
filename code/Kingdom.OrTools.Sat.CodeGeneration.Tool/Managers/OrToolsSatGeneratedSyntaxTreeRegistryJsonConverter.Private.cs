namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Code.Generation.Roslyn;
    using Newtonsoft.Json;

    /// <inheritdoc />
    internal partial class OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter
        : GeneratedSyntaxTreeRegistryJsonConverter<OrToolsSatGeneratedSyntaxTreeRegistry>
    {
        /// <summary>
        /// Private Default Constructor.
        /// </summary>
        private OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter()
        {
        }

        /// <summary>
        /// Gets a new <see cref="JsonConverter"/> instance.
        /// </summary>
        internal static OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter Converter
            => new OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter();
    }
}