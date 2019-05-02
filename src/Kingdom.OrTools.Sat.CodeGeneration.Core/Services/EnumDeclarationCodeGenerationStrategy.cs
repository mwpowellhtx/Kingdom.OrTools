using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

    internal class EnumDeclarationCodeGenerationStrategy
        : DescriptorCodeGenerationStrategyBase<EnumStatement, Identifier>
    {
        protected override EnumStatement Descriptor
        {
            get => base.Descriptor;
            set
            {
                _fields = null;
                base.Descriptor = value;
            }
        }

        internal static EnumDeclarationCodeGenerationStrategy Create(PackageStatement packageStatement, EnumStatement enumStatement)
            => new EnumDeclarationCodeGenerationStrategy(packageStatement, enumStatement);

        private EnumDeclarationCodeGenerationStrategy(PackageStatement packageStatement, EnumStatement enumStatement)
            : base(packageStatement, enumStatement)
        {
        }

        private ICollection<EnumFieldDescriptor> _fields;

        /// <summary>
        /// Gets the Fields <see cref="EnumFieldDescriptor"/> Collection. Yes, it is Internal.
        /// It is also an <see cref="ICollection{T}"/> for a reason because we want to be able
        /// to <see cref="ICollection{T}.Add"/> to it during the Visitor
        /// <see cref="EnumFieldDescriptor"/> Stack Reductions. Even though we could, it is
        /// also inappropriate to roll up anything based on the
        /// <see cref="DescriptorCodeGenerationStrategyBase{TDescriptor,TName}.Descriptor"/>.
        /// In other words, we are making a conscious decision here to integrate with the Visitor.
        /// </summary>
        internal ICollection<EnumFieldDescriptor> Fields => _fields ?? (_fields = new List<EnumFieldDescriptor>());

        private IEnumerable<SyntaxNodeOrToken> _memberSyntax;

        /// <summary>
        /// Returns the Generated <see cref="EnumMemberDeclarationSyntax"/> sans delimiters.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<SyntaxNodeOrToken> GetMemberSyntax()
        {
            AttributeSyntax GetAttribute(Identifier identifier)
            {
                // ReSharper disable once InconsistentNaming
                const string ParameterMemberName = nameof(ParameterMemberName);
                return Attribute(IdentifierName(ParameterMemberName))
                    .WithArgumentList(AttributeArgumentList(
                        SingletonSeparatedList(AttributeArgument(
                            LiteralExpression(StringLiteralExpression, Literal(identifier.Name))
                        ))
                    ));
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in Fields.ToArray())
            {
                var replaced = GetReplacedIdentifier(x.Name);

                yield return EnumMemberDeclaration(replaced.Name)
                    .WithAttributeLists(SingletonList(
                        AttributeList(SingletonSeparatedList(
                            GetAttribute(x.Name)
                        ))
                    ))
                    .WithEqualsValue(EqualsValueClause(
                            LiteralExpression(NumericLiteralExpression, Literal(x.Ordinal))
                        )
                    );
            }
        }

        private IEnumerable<SyntaxNodeOrToken> MemberSyntax => _memberSyntax ?? (_memberSyntax = GetMemberSyntax());

        protected override CompilationUnitSyntax GenerateCompilationUnit()
        {
            IEnumerable<MemberDeclarationSyntax> GetEnumDeclarations()
            {
                yield return EnumDeclaration(Descriptor.Name.Name)
                    .AddModifiers(Token(PublicKeyword))
                    .WithBaseList(BaseList(
                        SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(
                            PredefinedType(Token(LongKeyword))
                        ))
                    ))
                    .WithMembers(SeparatedList<EnumMemberDeclarationSyntax>(
                        MemberSyntax.CommaSeparated()
                    ));
            }

            return CompilationUnit().WithMembers(List(GetRange(
                    GenerateNameSpace(GetEnumDeclarations))
                ))
                .NormalizeWhitespace();
        }

        public static implicit operator CompilationUnitSyntax(EnumDeclarationCodeGenerationStrategy strategy)
            => strategy.GenerateCompilationUnit();
    }
}
