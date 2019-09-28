namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Newtonsoft.Json.Linq;

    internal partial class OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter
    {
        /// <inheritdoc />
        protected override JObject SerializeRegistry(OrToolsSatGeneratedSyntaxTreeRegistry registry)
        {
            var @object = base.SerializeRegistry(registry);
            const int fieldCount = 3;
            @object.Add(
                new JProperty(nameof(registry.GoogleOrToolsVersion)
                    , registry.GoogleOrToolsVersion.ToString(fieldCount))
            );
            return @object;
        }
    }
}
