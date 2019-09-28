namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;

    internal class RestartAlgorithmsClassDeclarationCodeGenerationStrategy
        : RestartAlgorithmsClassDeclarationCodeGenerationStrategyBase<ElementTypeIdentifierPath>
    {
        internal static RestartAlgorithmsClassDeclarationCodeGenerationStrategy Create(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            => new RestartAlgorithmsClassDeclarationCodeGenerationStrategy(packageStatement, normalFieldStatement);

        private RestartAlgorithmsClassDeclarationCodeGenerationStrategy(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            : base(packageStatement, normalFieldStatement)
        {
        }

        public static implicit operator CompilationUnitSyntax(RestartAlgorithmsClassDeclarationCodeGenerationStrategy strategy)
            => strategy.CompilationUnit;
    }
}
