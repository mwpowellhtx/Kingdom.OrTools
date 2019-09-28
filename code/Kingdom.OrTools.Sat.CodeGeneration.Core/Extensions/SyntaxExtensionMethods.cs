using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    internal static class SyntaxExtensionMethods
    {
        public static NameSyntax Qualify(this NameSyntax name, params SimpleNameSyntax[] others)
            => others.Aggregate(name, QualifiedName);

        public static IEnumerable<SyntaxNodeOrToken> CommaSeparated(this IEnumerable<SyntaxNodeOrToken> nodes)
            => nodes.Join(() => Token(CommaToken));
    }
}
