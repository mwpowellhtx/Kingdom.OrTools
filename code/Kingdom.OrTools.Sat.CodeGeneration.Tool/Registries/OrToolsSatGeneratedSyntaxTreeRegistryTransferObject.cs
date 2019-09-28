//using System;

//namespace Kingdom.OrTools.Sat.CodeGeneration
//{
//    using Code.Generation.Roslyn;

//    /// <inheritdoc />
//    internal class OrToolsSatGeneratedSyntaxTreeRegistryTransferObject : GeneratedSyntaxTreeRegistryTransferObject
//    {
//        /// <summary>
//        /// Gets or Sets the Google.OrTools <see cref="Version"/> string.
//        /// </summary>
//        public string GoogleOrToolsVersion { get; set; }

//        /// <summary>
//        /// Returns the Implicitly converted <paramref name="dto"/> as an
//        /// <see cref="OrToolsSatGeneratedSyntaxTreeRegistry"/>.
//        /// </summary>
//        /// <param name="dto"></param>
//        public static implicit operator OrToolsSatGeneratedSyntaxTreeRegistry(OrToolsSatGeneratedSyntaxTreeRegistryTransferObject dto)
//        {
//            var registry = new OrToolsSatGeneratedSyntaxTreeRegistry {GoogleOrToolsVersion = dto.GoogleOrToolsVersion.ParseVersion()};
//            void Add(GeneratedSyntaxTreeDescriptor x) => registry.Add(x);
//            dto.Descriptors.ForEach(Add);
//            return registry;
//        }
//    }
//}
