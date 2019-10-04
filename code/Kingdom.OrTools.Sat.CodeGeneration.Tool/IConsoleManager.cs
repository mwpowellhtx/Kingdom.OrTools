using System;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    // TODO: TBD: more of an interface could perhaps be exposed from the Console Manager packages ...
    // TODO: TBD: additionally, do the DefaultHelp* constants really need to be public for what they are? worst case internal, best case private?
    internal interface IConsoleManager : IDisposable
    {
        // TODO: TBD: re: above, so that we do not have to do this quite as much...
        void Run(out int errorLevel);

        bool TryParseOrShowHelp(params string[] args);
    }
}
