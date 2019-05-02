using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    internal static class SyntaxExtensionMethods
    {
        public static IEnumerable<SyntaxNodeOrToken> CommaSeparated(this IEnumerable<SyntaxNodeOrToken> nodes)
            => nodes.Join(() => Token(CommaToken));
    }
}
