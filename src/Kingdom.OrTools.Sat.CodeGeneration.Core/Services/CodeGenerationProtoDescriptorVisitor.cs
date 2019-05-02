﻿using System;
using System.Collections.Generic;
using Kingdom.Collections.Variants;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using static Protobuf.LabelKind;

    /*
     CompilationUnit()
        .WithMembers(
            SingletonList<MemberDeclarationSyntax>(
                EnumDeclaration("A")
                .WithMembers(
                    SeparatedList<EnumMemberDeclarationSyntax>(
                        new SyntaxNodeOrToken[]{
                            EnumMemberDeclaration(
                                Identifier("B"))
                            .WithEqualsValue(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(0)))),
                            Token(SyntaxKind.CommaToken),
                            EnumMemberDeclaration(
                                Identifier("C"))
                            .WithEqualsValue(
                                EqualsValueClause(
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(1))))}))))
        .NormalizeWhitespace()
     */
    /*
        namespace A.B.C
        {
        using D.E;
        public class TestParam : Param<int>
        {
        public TestParam() : base(default(int)) { }
        public TestParam(int value) : base(value) { }
        }
        }
     */
    /*
    CompilationUnit()
    .WithMembers(
        SingletonList<MemberDeclarationSyntax>(
            NamespaceDeclaration(
                QualifiedName(
                    QualifiedName(
                        IdentifierName("A"),
                        IdentifierName("B")
                    ),
                    IdentifierName("C")
                )
            )
            .WithUsings(
                SingletonList<UsingDirectiveSyntax>(
                    UsingDirective(
                        QualifiedName(
                            IdentifierName("D"),
                            IdentifierName("E")
                        )
                    )
                )
            )
            .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                    ClassDeclaration("TestParam")
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword)
                        )
                    )
                    .WithBaseList(
                        BaseList(
                            SingletonSeparatedList<BaseTypeSyntax>(
                                SimpleBaseType(
                                    GenericName(
                                        Identifier("Param")
                                    )
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                PredefinedType(
                                                    Token(SyntaxKind.IntKeyword)
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                    .WithMembers(
                        List<MemberDeclarationSyntax>(
                            new MemberDeclarationSyntax[]{
                                ConstructorDeclaration(
                                    Identifier("TestParam")
                                )
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PublicKeyword)
                                    )
                                )
                                .WithInitializer(
                                    ConstructorInitializer(
                                        SyntaxKind.BaseConstructorInitializer,
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    DefaultExpression(
                                                        PredefinedType(
                                                            Token(SyntaxKind.IntKeyword)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                                .WithBody(
                                    Block()
                                ),
                                ConstructorDeclaration(
                                    Identifier("TestParam")
                                )
                                .WithModifiers(
                                    TokenList(
                                        Token(SyntaxKind.PublicKeyword)
                                    )
                                )
                                .WithParameterList(
                                    ParameterList(
                                        SingletonSeparatedList<ParameterSyntax>(
                                            Parameter(
                                                Identifier("value")
                                            )
                                            .WithType(
                                                PredefinedType(
                                                    Token(SyntaxKind.IntKeyword)
                                                )
                                            )
                                        )
                                    )
                                )
                                .WithInitializer(
                                    ConstructorInitializer(
                                        SyntaxKind.BaseConstructorInitializer,
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    IdentifierName("value")
                                                )
                                            )
                                        )
                                    )
                                )
                                .WithBody(
                                    Block()
                                )
                            }
                        )
                    )
                )
            )
        )
    )
    .NormalizeWhitespace()
     */

    /// <summary>
    /// Represents a Code Generation Protocol Buffer Descriptor Visitor. We must approach this
    /// asset with some forward knowledge of the intended Protocol Buffer files. This exercise
    /// necessarily informs how much comprehension we must transliterate into CSharp Roslyn
    /// Tokens.
    /// </summary>
    /// <inheritdoc />
    internal class CodeGenerationProtoDescriptorVisitor : CodeGenerationProtoDescriptorVisitorBase
    {
        private PackageStatement PackageStatement { get; set; }

        protected override void EnterPackageStatement(PackageStatement statement)
        {
            /* There is nothing to Push Back onto the Stack here. But we do need to maintain
             * a reference to the Package for purposes of conversion to CSharp Namespace. */

            PackageStatement = statement;
        }

        protected override void ExitPackageStatement(PackageStatement statement)
        {
        }

        protected override void EnterEnumFieldOrdinal(long ordinal)
        {
            /* We do not have enough context to convert these to Literal Tokens.
             * Rather, that will be handled by a parent reduction. */
        }

        protected override void ExitEnumFieldOrdinal(long ordinal)
        {
        }

        // TODO: TBD: work in progress
        // TODO: TBD: I definitely think the stack based approach is accurate
        // TODO: TBD: however, how to bridge that at an appropriate level to Roslyn code generation is another matter entirely...
        // TODO: TBD: we need to identify appropriate level atomic bits I think...
        // TODO: TBD: Very helpful to identify the Roslyn CG calls: https://roslynquoter.azurewebsites.net/
        protected override void EnterIdentifier(Identifier identifier)
        {
            /* We do not have enough context to convert these to Literal Tokens.
             * Rather, that will be handled by a parent reduction. */
        }

        protected override void ExitIdentifier(Identifier identifier)
        {
        }

        /// <summary>
        /// We have everything we need at our disposal through <paramref name="descriptor"/> to
        /// convert that to the corresponding Roslyn tokens, including any heuristic or other
        /// attribute lists.
        /// </summary>
        /// <param name="descriptor"></param>
        protected override void EnterEnumFieldDescriptor(EnumFieldDescriptor descriptor)
        {
            Stack.PushBack(descriptor);
        }

        protected override void ExitEnumFieldDescriptor(EnumFieldDescriptor descriptor)
        {
            Stack.TryReduce((ref EnumDeclarationCodeGenerationStrategy a, EnumFieldDescriptor b) => a.Fields.Add(b));
        }

        protected override void EnterEnumStatement(EnumStatement statement)
        {
            Stack.PushBack(
                EnumDeclarationCodeGenerationStrategy.Create(PackageStatement, statement)
            );
        }

        protected override void ExitEnumStatement(EnumStatement statement)
        {
            Stack.TryReduce((ref IDictionary<Guid, CompilationUnitSyntax> a, EnumDeclarationCodeGenerationStrategy b) =>
                a.Add(Guid.NewGuid(), b)
            );
        }

        //IEnumerable<CompilationUnitSyntax> CompilationUnits => Stack.

        private IDictionary<Guid, CompilationUnitSyntax> _compilationUnits;

        internal IDictionary<Guid, CompilationUnitSyntax> CompilationUnits
            => _compilationUnits ?? (_compilationUnits
                   // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                   = new Dictionary<Guid, CompilationUnitSyntax> { }
               );

        protected override void EnterNormalFieldStatement(NormalFieldStatement statement)
        {
            void PushBack()
            {
                switch (statement.FieldType)
                {
                    case IVariant<ProtoType> _:
                        Stack.PushBack(
                            ProtoTypeParameterClassDeclarationCodeGenerationStrategy.Create(PackageStatement, statement)
                        );
                        break;
                    case IVariant<ElementTypeIdentifierPath> _:
                        Stack.PushBack(
                            ElementParameterClassDeclarationCodeGenerationStrategy.Create(PackageStatement, statement)
                        );
                        break;
                }
            }


            switch (statement.Label)
            {
                case Optional:
                    PushBack();
                    break;
                case Required:
                    // TODO: TBD: IVariant<ProtoType>, IVariant<OptionIdentifierPath>, ...
                    break;
                default:
                    // TODO: TBD: what to do about Repeated...
                    break;
            }
        }

        protected override void ExitNormalFieldStatement(NormalFieldStatement statement)
        {
            Stack.TryReduce((ref IDictionary<Guid, CompilationUnitSyntax> a, ProtoTypeParameterClassDeclarationCodeGenerationStrategy b) =>
                a.Add(Guid.NewGuid(), b)
            );

            Stack.TryReduce((ref IDictionary<Guid, CompilationUnitSyntax> a, ElementParameterClassDeclarationCodeGenerationStrategy b) =>
                a.Add(Guid.NewGuid(), b)
            );
        }

        public override void Visit(ProtoDescriptor descriptor)
        {
            Stack.PushBack(() => CompilationUnits);

            base.Visit(descriptor);
        }
    }
}