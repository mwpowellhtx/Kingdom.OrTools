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

    internal abstract class RestartAlgorithmsClassDeclarationCodeGenerationStrategyBase<T>
        : ParameterClassDeclarationCodeGenerationStrategyBase<T, IVariant<T>>
    {
        protected RestartAlgorithmsClassDeclarationCodeGenerationStrategyBase(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            : base(packageStatement, normalFieldStatement, BaseClassSpecification.RepeatedParameter)
        {
        }

        protected sealed override IEnumerable<UsingDirectiveSyntax> OuterUsingDirectives
        {
            get
            {
                // ReSharper disable InconsistentNaming
                const string System = nameof(System);
                const string Linq = nameof(Linq);
                // ReSharper restore InconsistentNaming

                yield return UsingDirective(
                    IdentifierName(System).Qualify(IdentifierName(Linq))
                );
            }
        }

        /// <summary>
        /// RestartAlgorithm
        /// </summary>
        protected const string RestartAlgorithm = nameof(RestartAlgorithm);

        protected sealed override IEnumerable<SyntaxNodeOrToken> TypeArgumentSyntax
        {
            get
            {
                // TODO: TBD: might consider re-factoring ProtoDescriptor comprehension to the base class...
                if (PackageStatement.Parent is ProtoDescriptor proto)
                {
                    var restartAlgorithmDescriptor = proto.Items.OfType<MessageStatement>()
                        .SelectMany(x => x.Items).OfType<EnumStatement>()
                        .SingleOrDefault(x => x.Name.Equals(RestartAlgorithm));

                    if (restartAlgorithmDescriptor == null)
                    {
                        throw new InvalidOperationException($"Unable to locate {RestartAlgorithm} enum specification.");
                    }

                    yield return IdentifierName(restartAlgorithmDescriptor.Name.Name);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid {nameof(PackageStatement)} instance.");
                }
            }
        }

        /// <summary>
        /// Gets the Constructor Specifications assuming that At Least One such
        /// <see cref="RestartAlgorithm"/> must be specified.
        /// </summary>
        /// <see cref="!:https://groups.google.com/forum/#!topic/or-tools-discuss/xlvn_2kOONQ">
        /// Question pending on the forums as to whether ZERO or more algorithms may be
        /// specified. Which would result in slightly different code being generated.</see>
        /// <inheritdoc />
        protected override IEnumerable<ConstructorSpecification> CtorSpecifications
        {
            get
            {
                {
                    // ReSharper disable InconsistentNaming
                    const string algorithm = nameof(algorithm);
                    const string algorithms = nameof(algorithms);
                    const string Concat = nameof(Enumerable.Concat);
                    // ReSharper restore InconsistentNaming

                    /* Essentially specifying, whitespace normalization notwithstanding:
                     * public <Parameter/>(T algorithm, params T[] others) : base(new[] {algorithm}.Concat(other))
                     * {
                     * } */

                    yield return new ConstructorSpecification
                    {
                        Parameters = GetRange<SyntaxNodeOrToken>(
                            Parameter(Identifier(algorithm))
                                .WithType(IdentifierName(RestartAlgorithm))
                            , Parameter(Identifier(algorithms))
                                .WithModifiers(TokenList(Token(ParamsKeyword)))
                                .WithType(ArrayType(IdentifierName(RestartAlgorithm))
                                    .WithRankSpecifiers(SingletonList(
                                        ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(
                                            OmittedArraySizeExpression()
                                        ))
                                    ))
                                )
                        ),
                        InitializerArguments = GetRange<SyntaxNodeOrToken>(
                            Argument(InvocationExpression(
                                    MemberAccessExpression(
                                        SimpleMemberAccessExpression
                                        , ImplicitArrayCreationExpression(
                                            InitializerExpression(
                                                ArrayInitializerExpression
                                                , SingletonSeparatedList<ExpressionSyntax>(IdentifierName(algorithm))
                                            )
                                        )
                                        , IdentifierName(Concat)
                                    )
                                )
                                .WithArgumentList(ArgumentList(
                                    SingletonSeparatedList(Argument(IdentifierName(algorithms)))
                                ))
                            )
                            , Argument(DescriptorOrdinalExpressionSyntax)
                        )
                    };
                }
            }
        }
    }
}
