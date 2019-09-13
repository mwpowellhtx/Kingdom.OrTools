namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using static NConsole.Options.ConsoleManager;

    /// <summary>
    /// The Program class.
    /// </summary>
    /// <remarks>There is no reason for this to be Non Static.</remarks>
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
