using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Collections.Variants;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;
    using static Characters;
    using static StringSplitOptions;

    internal class DefaultRestartAlgorithmsClassDeclarationCodeGenerationStrategy
        : RestartAlgorithmsClassDeclarationCodeGenerationStrategyBase<ProtoType>
    {
        internal static DefaultRestartAlgorithmsClassDeclarationCodeGenerationStrategy Create(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            => new DefaultRestartAlgorithmsClassDeclarationCodeGenerationStrategy(packageStatement, normalFieldStatement);

        private DefaultRestartAlgorithmsClassDeclarationCodeGenerationStrategy(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            : base(packageStatement, normalFieldStatement)
        {
        }

        protected override IEnumerable<UsingDirectiveSyntax> InnerUsingDirectives
        {
            get
            {
                yield return UsingDirective(IdentifierName(RestartAlgorithm))
                    .WithStaticKeyword(Token(StaticKeyword));
            }
        }

        private IEnumerable<Identifier> DefaultFieldOptionOrDefaultValues
        {
            get
            {
                var option = DefaultFieldOptionOrDefault;
                if (option is IVariant<string> x)
                {
                    var values = x.Value.Split(GetRange(Comma).ToArray(), RemoveEmptyEntries);
                    if (!values.Any())
                    {
                        throw new InvalidOperationException(
                            $"Expecting At Least One {RestartAlgorithm}"
                            + " value informing the Default Constructor."
                        );
                    }

                    foreach (var y in values)
                    {
                        yield return GetReplacedIdentifier(y);
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "Unable to properly interpret expected Default Field Option"
                        + $" as a '{typeof(IVariant<string>).FullName}'."
                    );
                }
            }
        }

        protected override IEnumerable<ConstructorSpecification> CtorSpecifications
        {
            get
            {
                {
                    yield return new ConstructorSpecification
                    {
                        InitializerKeyword = ThisConstructorInitializer,
                        InitializerArguments = GetRange(
                            DefaultFieldOptionOrDefaultValues
                                .Select(x => Argument(IdentifierName(x.Name)))
                                .Select(x => (SyntaxNodeOrToken) x).ToArray()
                        )
                    };
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var x in base.CtorSpecifications)
                {
                    yield return x;
                }
            }
        }

        public static implicit operator CompilationUnitSyntax(DefaultRestartAlgorithmsClassDeclarationCodeGenerationStrategy strategy)
            => strategy.CompilationUnit;
    }
}
