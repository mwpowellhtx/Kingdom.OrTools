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
    using SharpSyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    internal abstract class ParameterClassDeclarationCodeGenerationStrategyBase<TVariantType, TFieldType>
        : DescriptorCodeGenerationStrategyBase<NormalFieldStatement, Identifier>
        where TFieldType : class, IVariant<TVariantType>
    {
        protected enum BaseClassSpecification
        {
            Parameter,
            RepeatedParameter
        }

        protected TFieldType DescriptorFieldType
            => Descriptor.FieldType as TFieldType
               ?? throw new InvalidOperationException(
                   $"'{Descriptor.GetType().FullName}.{nameof(Descriptor.FieldType)}'"
                   + $" '{Descriptor.FieldType.GetType().FullName}' unsupported."
               );

        private LiteralExpressionSyntax _descriptorOrdinalExpressionSyntax;

        /// <summary>
        /// Gets the Ordinal <see cref="LiteralExpressionSyntax"/> value.
        /// </summary>
        /// <see cref="FieldStatementBase{T}.Number"/>
        protected LiteralExpressionSyntax DescriptorOrdinalExpressionSyntax
            => _descriptorOrdinalExpressionSyntax ?? (_descriptorOrdinalExpressionSyntax
                   = LiteralExpression(NumericLiteralExpression, Literal(Descriptor.Number))
               );

        protected override NormalFieldStatement Descriptor
        {
            get => base.Descriptor;
            set
            {
                _className = null;
                _defaultFieldOptionOrDefault = null;
                _descriptorOrdinalExpressionSyntax = null;
                base.Descriptor = value;
            }
        }

        private Identifier _className;

        /// <summary>
        /// Gets the Parameter Base Class <see cref="Identifier"/>.
        /// </summary>
        private Identifier ParameterBaseClassIdentifier { get; }

        /// <summary>
        /// Gets the Descriptor Full Class Name.
        /// </summary>
        /// <see cref="ParameterBaseClassIdentifier"/>
        private Identifier DescriptorFullClassName
        {
            get
            {
                Identifier GetClassNameIdentifier(Identifier identifier)
                {
                    // ReSharper disable once InconsistentNaming
                    identifier = GetReplacedIdentifier(identifier);
                    identifier.Name = $"{identifier.Name}{ParameterBaseClassIdentifier.Name}";
                    return identifier;
                }

                return _className ?? (_className = GetClassNameIdentifier(Descriptor.Name));
            }
        }

        protected ParameterClassDeclarationCodeGenerationStrategyBase(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement
            , BaseClassSpecification baseClass = BaseClassSpecification.Parameter)
            : base(packageStatement, normalFieldStatement)
        {
            ParameterBaseClassIdentifier = baseClass.ToString();
        }

        private IVariant _defaultFieldOptionOrDefault;

        /// <summary>
        /// Literally, Gets the Default <see cref="FieldOption"/> if it exists.
        /// </summary>
        protected IVariant DefaultFieldOptionOrDefault
        {
            get
            {
                const string @default = nameof(@default);

                return _defaultFieldOptionOrDefault ?? (_defaultFieldOptionOrDefault
                           = Descriptor.Options.FirstOrDefault(
                               x => x.Name.Any() && x.Name.Last().Equals(@default))?.Value
                       );
            }
        }

        /// <summary>
        /// Specification which captures key aspects of a typical Constructor Declaration.
        /// </summary>
        protected class ConstructorSpecification
        {
            /// <summary>
            /// Gets or Sets the <see cref="SharpSyntaxKind"/> ModifierKeyword.
            /// Default value is <see cref="PublicKeyword"/>.
            /// </summary>
            /// <see cref="PublicKeyword"/>
            public SharpSyntaxKind ModifierKeyword { get; set; } = PublicKeyword;

            /// <summary>
            /// Gets or Sets the Parameters. Focus on providing these Tokens sans Comma tokens.
            /// </summary>
            public IEnumerable<SyntaxNodeOrToken> Parameters { get; set; } = new List<SyntaxNodeOrToken>();

            /// <summary>
            /// Gets or Sets the <see cref="SharpSyntaxKind"/> InitializerKeyword.
            /// Default value is <see cref="BaseConstructorInitializer"/>.
            /// </summary>
            /// <see cref="BaseConstructorInitializer"/>
            public SharpSyntaxKind InitializerKeyword { get; set; } = BaseConstructorInitializer;

            /// <summary>
            /// Gets or Sets the InitializerArguments.
            /// Focus on providing these Tokens sans Comma Tokens.
            /// </summary>
            public IEnumerable<SyntaxNodeOrToken> InitializerArguments { get; set; } = new List<SyntaxNodeOrToken>();

            /// <summary>
            /// Allowing for compelling Tuple-like deconstruction.
            /// </summary>
            /// <param name="modifierKeyword"></param>
            /// <param name="parameters"></param>
            /// <param name="initializerKeyword"></param>
            /// <param name="initializerArguments"></param>
            public void Deconstruct(out SharpSyntaxKind modifierKeyword
                , out IEnumerable<SyntaxNodeOrToken> parameters
                , out SharpSyntaxKind initializerKeyword
                , out IEnumerable<SyntaxNodeOrToken> initializerArguments
            )
            {
                modifierKeyword = ModifierKeyword;
                parameters = Parameters;
                initializerKeyword = InitializerKeyword;
                initializerArguments = InitializerArguments;
            }
        }

        /// <summary>
        /// Gets the <see cref="ConstructorSpecification"/> instances.
        /// </summary>
        protected abstract IEnumerable<ConstructorSpecification> CtorSpecifications { get; }

        private IEnumerable<MemberDeclarationSyntax> GetClassMembers(Identifier className)
        {
            SyntaxToken GetIdentifierToken() => Identifier(className.Name);

            foreach (var (modifierKeyword, parameters, initializerKeyword, initializerArguments) in CtorSpecifications)
            {
                var ctorDecl = ConstructorDeclaration(GetIdentifierToken())
                    .WithModifiers(TokenList(Token(modifierKeyword)))
                    ;

                // ReSharper disable PossibleMultipleEnumeration
                if (parameters.Any())
                {
                    ctorDecl = ctorDecl.WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(
                        parameters.CommaSeparated()
                    )));
                }
                // ReSharper restore PossibleMultipleEnumeration

                /* It's a minor issue, but the Ctor Initializer Normalized Whitespace is kind of abnormal:
                 * https://github.com/dotnet/roslyn/issues/35371. Cyrus was not especially helpful, as per
                 * usual. So filed the issue for the team to follow up on. Originally called, "Ctor Initializer
                 * normalization is inconsistent with neighboring areas like class decl lists." */

                ctorDecl = ctorDecl.WithInitializer(ConstructorInitializer(
                    initializerKeyword
                    , ArgumentList(SeparatedList<ArgumentSyntax>(
                        initializerArguments.CommaSeparated()
                    ))
                ));

                yield return ctorDecl.WithBody(Block());
            }
        }

        /// <summary>
        /// Gets the <see cref="SyntaxNodeOrToken"/> informing the Class Generic Type Argument
        /// List.
        /// </summary>
        protected abstract IEnumerable<SyntaxNodeOrToken> TypeArgumentSyntax { get; }

        protected delegate ExpressionSyntax ConvertVariantToExpression(IVariant variant);

        // TODO: TBD: "value" potentially to be informed by the "default" field option (if any)...
        protected override IEnumerable<MemberDeclarationSyntax> NameSpaceMembers
        {
            get
            {
                IEnumerable<SyntaxTrivia> GetClassDeclarationLeadingTrivia()
                    => GetNormalFieldStatementOrdinalMetricsTriviaStrings(Descriptor).Select(Comment);

                SyntaxTriviaList GetClassDeclarationTrailingTrivia() => TriviaList();

                // ReSharper disable once InconsistentNaming
                const string ParameterName = nameof(ParameterName);

                var parameterNameAttribute = Attribute(IdentifierName(ParameterName))
                    .WithArgumentList(AttributeArgumentList(SingletonSeparatedList(
                        AttributeArgument(
                            LiteralExpression(StringLiteralExpression
                                , Literal(Descriptor.Name.Name))
                        )
                    )));

                var classDecl = ClassDeclaration(Identifier(DescriptorFullClassName.Name))
                    .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(parameterNameAttribute))))
                    .WithModifiers(TokenList(Token(PublicKeyword)))
                    .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(
                        SimpleBaseType(GenericName(
                                Identifier(ParameterBaseClassIdentifier.Name)
                            )
                            .WithTypeArgumentList(TypeArgumentList(
                                SeparatedList<TypeSyntax>(TypeArgumentSyntax.CommaSeparated())
                            ))
                        )
                    )))
                    .WithMembers(List(
                        GetClassMembers(DescriptorFullClassName)
                    ));

                var classDeclWithTrivia = classDecl.WithLeadingTrivia(
                    GetClassDeclarationLeadingTrivia()
                );

                yield return classDeclWithTrivia;
            }
        }
    }
}
