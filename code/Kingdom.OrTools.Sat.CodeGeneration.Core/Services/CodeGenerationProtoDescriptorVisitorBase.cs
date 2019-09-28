using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using Protobuf.Collections;

    /// <summary>
    /// Each Visited Node in the <see cref="ProtoDescriptor"/> incurs an Enter and an Exit
    /// callback. You are free to Push as many states onto the Back of the Stack as is
    /// necessary to properly handle each node, or none at all if that is so desired. Be sure
    /// that the bounding Exit handler reduces an appropriate number of states in order to
    /// properly resolve the visitation. The general rule of thumb is that if there were N states
    /// Pushed, then there should be N-1 reductions for the Visit itself. There is also possibly
    /// a reduction for the Parent Visit state, as necessary.
    /// </summary>
    /// <inheritdoc />
    internal abstract class CodeGenerationProtoDescriptorVisitorBase : ProtoDescriptorVisitorBase
    {
        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        protected AbstractSyntaxTreeStack<List<MemberDeclarationSyntax>> Stack { get; }
            = new AbstractSyntaxTreeStack<List<MemberDeclarationSyntax>> { };

        protected abstract void EnterPackageStatement(PackageStatement statement);

        protected abstract void ExitPackageStatement(PackageStatement statement);

        protected override void VisitPackageStatement(PackageStatement statement)
        {
            EnterPackageStatement(statement);

            base.VisitPackageStatement(statement);

            ExitPackageStatement(statement);
        }

        //// TODO: TBD: we may want these after all... but not necessary for enums themselves...
        //protected abstract void EnterLabel(LabelKind value);

        //protected abstract void ExitLabel(LabelKind value);

        //protected sealed override void VisitLabel(LabelKind value)
        //{
        //    EnterLabel(value);
        //    base.VisitLabel(value);
        //    ExitLabel(value);
        //}

        protected abstract void EnterIdentifier(Identifier identifier);

        protected abstract void ExitIdentifier(Identifier identifier);

        protected sealed override void VisitIdentifier(Identifier identifier)
        {
            EnterIdentifier(identifier);

            base.VisitIdentifier(identifier);

            ExitIdentifier(identifier);
        }

        protected abstract void EnterEnumFieldOrdinal(long ordinal);

        protected abstract void ExitEnumFieldOrdinal(long ordinal);

        protected sealed override void VisitEnumFieldOrdinal(long ordinal)
        {
            EnterEnumFieldOrdinal(ordinal);

            base.VisitEnumFieldOrdinal(ordinal);

            ExitEnumFieldOrdinal(ordinal);
        }

        protected abstract void EnterEnumFieldDescriptor(EnumFieldDescriptor descriptor);

        protected abstract void ExitEnumFieldDescriptor(EnumFieldDescriptor descriptor);

        protected sealed override void VisitEnumFieldDescriptor(EnumFieldDescriptor descriptor)
        {
            EnterEnumFieldDescriptor(descriptor);

            base.VisitEnumFieldDescriptor(descriptor);

            ExitEnumFieldDescriptor(descriptor);
        }

        protected abstract void EnterEnumStatement(EnumStatement statement);

        protected abstract void ExitEnumStatement(EnumStatement statement);

        protected sealed override void VisitEnumStatement(EnumStatement statement)
        {
            EnterEnumStatement(statement);

            base.VisitEnumStatement(statement);

            ExitEnumStatement(statement);
        }

        protected abstract void EnterNormalFieldStatement(NormalFieldStatement statement);

        protected abstract void ExitNormalFieldStatement(NormalFieldStatement statement);

        protected override void VisitNormalFieldStatement(NormalFieldStatement statement)
        {
            EnterNormalFieldStatement(statement);

            base.VisitNormalFieldStatement(statement);

            ExitNormalFieldStatement(statement);
        }
    }
}
