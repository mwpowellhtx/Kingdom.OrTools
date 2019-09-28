namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using NConsole.Options;

    internal interface ICodeGenerationConsoleManager : IConsoleManager
    {
        /// <summary>
        /// Gets whether DebugMessages should be written.
        /// </summary>
        Switch DebugMessagesSwitch { get; }

        /// <summary>
        /// Gets whether Version was requested.
        /// </summary>
        Switch VersionSwitch { get; }

        /// <summary>
        /// Gets whether `Google.OrTools´ Version was requested.
        /// </summary>
        Switch GoogleOrToolsVersionSwitch { get; }

        ///// <summary>
        ///// Gets the Google.OrTools SAT Parameters Protocol Buffer Specification Variable.
        ///// </summary>
        //Variable<string> GoogleOrToolsSatParametersProtocolBufferSpecVar { get; }

        /// <summary>
        /// Gets the Output Directory Variable.
        /// </summary>
        Variable<string> OutputDirectoryVar { get; }

        /// <summary>
        /// Gets the Generated Code Registry File Variable.
        /// </summary>
        Variable<string> GeneratedCodeRegistryFileVar { get; }
    }
}
