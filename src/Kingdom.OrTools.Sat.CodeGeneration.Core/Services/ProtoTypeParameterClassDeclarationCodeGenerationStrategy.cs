using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Collections.Variants;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;
    using static Protobuf.ProtoType;
    using SharpSyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    internal class ProtoTypeParameterClassDeclarationCodeGenerationStrategy
        : ParameterClassDeclarationCodeGenerationStrategyBase<ProtoType, IVariant<ProtoType>>
    {
        internal static ProtoTypeParameterClassDeclarationCodeGenerationStrategy Create(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            => new ProtoTypeParameterClassDeclarationCodeGenerationStrategy(packageStatement, normalFieldStatement);

        private ProtoTypeParameterClassDeclarationCodeGenerationStrategy(
            PackageStatement packageStatement, NormalFieldStatement normalFieldStatement)
            : base(packageStatement, normalFieldStatement)
        {
        }

        // TODO: TBD: we may yet refactor this to the base/intermediate strategy class...
        // TODO: TBD: however, for now, let's focus on just this case.
        /// <summary>
        /// Gets the mapping of <see cref="ProtoType"/> to <see cref="SharpSyntaxKind"/>,
        /// as informed by the Protocol Buffer Language Guide &quot;Scalar Value Types&quot;.
        /// Yes, we know this is &quot;Version 3&quot;, however, this closely follows the
        /// &quot;Version 2&quot; specification, except as applies for CSharp.
        /// </summary>
        /// <see cref="!:http://developers.google.com/protocol-buffers/docs/proto2#scalar"/>
        /// <see cref="!:http://developers.google.com/protocol-buffers/docs/proto3#scalar"/>
        /// <see cref="!:http://developers.google.com/protocol-buffers/docs/reference/proto2-spec"/>
        /// <see cref="!:http://developers.google.com/protocol-buffers/docs/reference/proto3-spec"/>
        private static IDictionary<ProtoType, SharpSyntaxKind> VariantTypeSyntaxKindMap { get; }
            = new Dictionary<ProtoType, SharpSyntaxKind>
            {
                {Bool, BoolKeyword},
                {Float, FloatKeyword},
                {Double, DoubleKeyword},
                {Int32, IntKeyword},
                {Int64, LongKeyword},
                {SInt32, IntKeyword},
                {SInt64, LongKeyword},
                {SFixed32, IntKeyword},
                {SFixed64, LongKeyword},
                {UInt32, UIntKeyword},
                {UInt64, ULongKeyword},
                {Fixed32, UIntKeyword},
                {Fixed64, ULongKeyword},
                {String, StringKeyword}
            };

        protected override IEnumerable<SyntaxNodeOrToken> TypeArgumentSyntax
        {
            get { yield return PredefinedType(Token(VariantTypeSyntaxKindMap[DescriptorFieldType.Value])); }
        }

        private static ExpressionSyntax ConvertBooleanVariantToExpression(IVariant variant)
            => LiteralExpression((bool) variant.Value ? TrueLiteralExpression : FalseLiteralExpression);

        private static ExpressionSyntax ConvertFloatVariantToExpression(IVariant variant)
        {
            if (variant is IVariant<long>)
            {
                return LiteralExpression(NumericLiteralExpression, Literal(variant.Value.ReinterpretFloat()));
            }

            var x = (IVariant<float>) variant;

            if (float.IsPositiveInfinity(x.Value))
            {
                return MemberAccessExpression(
                    SimpleMemberAccessExpression
                    , PredefinedType(Token(FloatKeyword))
                    , IdentifierName($"float.{nameof(float.PositiveInfinity)}")
                );
            }

            if (float.IsNegativeInfinity(x.Value))
            {
                return MemberAccessExpression(
                    SimpleMemberAccessExpression
                    , PredefinedType(Token(FloatKeyword))
                    , IdentifierName($"float.{nameof(float.NegativeInfinity)}")
                );
            }

            if (float.IsNaN(x.Value))
            {
                return MemberAccessExpression(
                    SimpleMemberAccessExpression
                    , PredefinedType(Token(FloatKeyword))
                    , IdentifierName($"float.{nameof(float.NaN)}")
                );
            }

            // Literal Expressions involving Float are fine.
            return LiteralExpression(NumericLiteralExpression, Literal(x.Value));
        }

        private static ExpressionSyntax ConvertDoubleVariantToExpression(IVariant variant)
        {
            if (variant is IVariant<long>)
            {
                return LiteralExpression(NumericLiteralExpression, Literal(variant.Value.ReinterpretDouble()));
            }

            var x = (IVariant<double>) variant;

            if (double.IsPositiveInfinity(x.Value))
            {
                return MemberAccessExpression(
                    SimpleMemberAccessExpression
                    , PredefinedType(Token(DoubleKeyword))
                    , IdentifierName($"double.{nameof(double.PositiveInfinity)}")
                );
            }

            if (double.IsNegativeInfinity(x.Value))
            {
                return MemberAccessExpression(
                    SimpleMemberAccessExpression
                    , PredefinedType(Token(DoubleKeyword))
                    , IdentifierName($"double.{nameof(double.NegativeInfinity)}")
                );
            }

            if (double.IsNaN(x.Value))
            {
                return MemberAccessExpression(
                    SimpleMemberAccessExpression
                    , PredefinedType(Token(DoubleKeyword))
                    , IdentifierName($"double.{nameof(double.NaN)}")
                );
            }

            //// https://github.com/dotnet/roslyn/issues/35446 / Literal "interpretations" not working correctly
            //return LiteralExpression(NumericLiteralExpression, Literal(x.Value));
            return IdentifierName($"{x.Value:R}D");
        }

        private static ExpressionSyntax ConvertIntegerVariantToExpression(IVariant variant)
            => LiteralExpression(NumericLiteralExpression, Literal(variant.Value.ReinterpretInteger()));

        private static ExpressionSyntax ConvertUnsignedIntegerVariantToExpression(IVariant variant)
            => LiteralExpression(NumericLiteralExpression, Literal(variant.Value.ReinterpretUnsignedInteger()));

        private static ExpressionSyntax ConvertLongVariantToExpression(IVariant variant)
            => LiteralExpression(NumericLiteralExpression, Literal(variant.Value.ReinterpretLong()));

        private static ExpressionSyntax ConvertUnsignedLongVariantToExpression(IVariant variant)
            => LiteralExpression(NumericLiteralExpression, Literal(variant.Value.ReinterpretUnsignedLong()));

        private static ExpressionSyntax ConvertStringVariantToExpression(IVariant variant)
            => LiteralExpression(StringLiteralExpression, Literal(variant.Value.ReinterpretString()));

        // TODO: TBD: Saving the IdentifierPath NormalFieldStatement FieldType use case for another derived class...
        // TODO: TBD: which class should even verify that an EnumStatement declaration has occurred, etc, etc, etc...
        // TODO: TBD: which is a potentially long list of 'etc' verification, but we will cross that bridge in a next step...
        /// <summary>
        /// Known Constant Primitive application types include <see cref="bool"/>,
        /// <see cref="long"/>, <see cref="double"/>, and <see cref="string"/>.
        /// </summary>
        private static readonly IDictionary<ProtoType, ConvertVariantToExpression> DefaultValueConverters
            = new Dictionary<ProtoType, ConvertVariantToExpression>
            {
                {Bool, ConvertBooleanVariantToExpression},
                {Float, ConvertFloatVariantToExpression},
                {Double, ConvertDoubleVariantToExpression},
                {Int32, ConvertIntegerVariantToExpression},
                {Int64, ConvertLongVariantToExpression},
                {SInt32, ConvertIntegerVariantToExpression},
                {SInt64, ConvertLongVariantToExpression},
                {SFixed32, ConvertIntegerVariantToExpression},
                {SFixed64, ConvertLongVariantToExpression},
                {UInt32, ConvertUnsignedIntegerVariantToExpression},
                {UInt64, ConvertUnsignedLongVariantToExpression},
                {Fixed32, ConvertUnsignedIntegerVariantToExpression},
                {Fixed64, ConvertUnsignedLongVariantToExpression},
                {String, ConvertStringVariantToExpression}
            };

        /// <summary>
        /// Factory Gets a new Predefined Type based upon the
        /// <see cref="ParameterClassDeclarationCodeGenerationStrategyBase{TVariantType,TFieldType}.DescriptorFieldType"/>.
        /// </summary>
        /// <see cref="VariantTypeSyntaxKindMap"/>
        private TypeSyntax CtorParameterTypeSyntax => PredefinedType(
            Token(VariantTypeSyntaxKindMap[DescriptorFieldType.Value])
        );

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
                                : Argument(DefaultValueConverters[DescriptorFieldType.Value].Invoke(defaultOption));

                        SyntaxNodeOrToken GetDefaultExpression() =>
                            Argument(DefaultExpression(CtorParameterTypeSyntax));

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
                                .WithType(CtorParameterTypeSyntax)
                        ),
                        InitializerArguments = GetRange<SyntaxNodeOrToken>(
                            Argument(IdentifierName(value))
                            , Argument(DescriptorOrdinalExpressionSyntax)
                        )
                    };
                }
            }
        }

        public static implicit operator CompilationUnitSyntax(ProtoTypeParameterClassDeclarationCodeGenerationStrategy strategy)
            => strategy.CompilationUnit;
    }
}
