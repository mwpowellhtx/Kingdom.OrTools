using System;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Newtonsoft.Json.Linq;
    using Registry = OrToolsSatGeneratedSyntaxTreeRegistry;

    internal partial class OrToolsSatGeneratedSyntaxTreeRegistryJsonConverter
    {
        /// <inheritdoc />
        public override Registry DeserializeRegistry(JObject @object, Registry registry)
        {
            Version Parse(string s) => Version.Parse(s);
            var properties = @object.Properties().ToDictionary(x => x.Name);
            registry.GoogleOrToolsVersion = Parse(properties[nameof(registry.GoogleOrToolsVersion)].Value.Value<string>());
            return base.DeserializeRegistry(@object, registry);
        }
    }
}
