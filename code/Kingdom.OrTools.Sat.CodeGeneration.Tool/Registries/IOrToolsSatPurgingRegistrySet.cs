using System;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Code.Generation.Roslyn;

    /// <inheritdoc />
    internal interface IOrToolsSatPurgingRegistrySet : IPurgingRegistrySet
    {
        /// <summary>
        /// Gets or Sets the Google.OrTools Version. In addition to the set of generated code
        /// itself, it is important that we track the OrTools version, because when we track
        /// with newer versions, we want to generate a new set of code based on the updated
        /// OrTools version.
        /// </summary>
        Version GoogleOrToolsVersion { get; set; }
    }

    /// <inheritdoc cref="IOrToolsSatPurgingRegistrySet" />
    internal interface IOrToolsSatPurgingRegistrySet<T> : IOrToolsSatPurgingRegistrySet, IPurgingRegistrySet<T>
    {
    }
}
