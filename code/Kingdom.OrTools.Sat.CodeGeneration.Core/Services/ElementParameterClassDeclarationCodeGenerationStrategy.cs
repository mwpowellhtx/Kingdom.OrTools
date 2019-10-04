using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Collections.Variants;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;
    using static Characters;
    using static String;
    using SharpSyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    internal class ElementParameterClassDeclarationCodeGenerationStrategy
        : ParameterClassDeclarationCodeGenerationStrategyBase<ElementTypeIdentifierPath
            , IVariant<ElementTypeIdentifierPath>>
    {
        internal static ElementParameterClassDeclarationCodeGenerationStrategy Create(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            => new ElementParameterClassDeclarationCodeGenerationStrategy(packageStatement, normalFieldStatement);

        private ElementParameterClassDeclarationCodeGenerationStrategy(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            : base(packageStatement, normalFieldStatement)
        {
        }

        // TODO: TBD: the full path, whatever that is, may be fine...
        // TODO: TBD: may decided to just take the Last... but we'll see how it works first...
        private NameSyntax DescriptorFieldTypeSyntax => IdentifierName(
            Join($"{Dot}", DescriptorFieldType.Value.Select(x => x.Name))
        );

        protected override IEnumerable<SyntaxNodeOrToken> TypeArgumentSyntax
        {
            get { yield return DescriptorFieldTypeSyntax; }
        }

        private ExpressionSyntax ConvertElementTypeIdentifierPathToDefault(IVariant defaultFieldOption)
        {
            //// TODO: TBD: this may inform a static usage of the enum type.
            //var fieldTypePath = DescriptorFieldType.Value;

            //// TODO: TBD: for now we may simply rely upon the Default being furnished.
            //// TODO: TBD: later on we may find it necessary in order to break out and verify/validate more of the scope...
            //if (Descriptor.Parent is MessageStatement message)
            //{
            //    message.Items
            //}

            if (defaultFieldOption is IVariant<IdentifierPath> identifierPath)
            {
                var replacedIdentifier = GetReplacedIdentifier(identifierPath.Value.Last());
                return IdentifierName(replacedIdentifier.Name);
            }

            throw new ArgumentException($"Unable to discern variant value.", nameof(defaultFieldOption));
        }

        protected override IEnumerable<ConstructorSpecification> CtorSpecifications
        {
            get
            {
                {
                    IEnumerable<SyntaxNodeOrToken> GetThisCtorInitializerArguments()
                    {
                        // This works because we are dealing with a Struct.
                        SyntaxNodeOrToken? GetConvertedDefault(IVariant defaultOption)
                            => defaultOption == null
                                ? null
                                : Argument(ConvertElementTypeIdentifierPathToDefault(defaultOption));

                        SyntaxNodeOrToken GetDefaultExpression() => Argument(DefaultExpression(
                            IdentifierName(DescriptorFieldType.Value.Last().Name)
                        ));

                        // TODO: TBD: this bit may well be refactored to base class anticipating subsequent variants of the service...
                        return GetRange(GetConvertedDefault(DefaultFieldOptionOrDefault) ?? GetDefaultExpression());
                    }

                    yield return new ConstructorSpecification
                    {
                        InitializerKeyword = ThisConstructorInitializer,
                        InitializerArguments = GetThisCtorInitializerArguments()
                    };
                }

                {
                    const string value = nameof(value);

                    // TODO: as such, can probably refactor these to private scope...
                    yield return new ConstructorSpecification
                    {
                        InitializerKeyword = BaseConstructorInitializer,
                        Parameters = GetRange<SyntaxNodeOrToken>(
                            Parameter(Identifier(value))
                                .WithType(DescriptorFieldTypeSyntax)
                        ),
                        InitializerArguments = GetRange<SyntaxNodeOrToken>(
                            Argument(IdentifierName(value))
                            , Argument(DescriptorOrdinalExpressionSyntax)
                        )
                    };
                }
            }
        }

        protected override IEnumerable<UsingDirectiveSyntax> InnerUsingDirectives => GetRange(
            UsingDirective(DescriptorFieldTypeSyntax)
                .WithStaticKeyword(Token(StaticKeyword))
        );

        public static implicit operator CompilationUnitSyntax(ElementParameterClassDeclarationCodeGenerationStrategy strategy)
            => strategy.CompilationUnit;
    }
}
