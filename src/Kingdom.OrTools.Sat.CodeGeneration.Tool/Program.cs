namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using static NConsole.Options.ConsoleManager;

    /// <summary>
    /// The Program class.
    /// </summary>
    /// <remarks>Evaluate whether there should be any tooling updates, never mind deeper into the
    /// Protocol Buffer ANTLR parser concerns, based on whether the `sat_parameters.proto´ has
    /// updated at all since the most recent baselines.</remarks>
    /// <see cref="!:https://github.com/google/or-tools/blob/stable/ortools/sat/sat_parameters.proto"/>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            using (var instance = CodeGenerationConsoleManager.Instance)
            {
                // ReSharper disable once InvertIf
                if (instance.TryParseOrShowHelp(args))
                {
                    instance.Run(out var errorLevel);
                    return errorLevel;
                }
            }

            // TODO: TBD: return default here? or another code?
            return DefaultErrorLevel;
        }
    }
}
