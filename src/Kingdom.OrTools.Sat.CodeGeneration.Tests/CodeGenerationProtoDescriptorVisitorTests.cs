using System;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Protobuf;
    using Xunit;
    using Xunit.Abstractions;
    using VisitorVerificationAction = Action<CodeGenerationProtoDescriptorVisitor, Protobuf.ProtoDescriptor>;

    public class CodeGenerationProtoDescriptorVisitorTests : TestFixtureBase
    {
        public CodeGenerationProtoDescriptorVisitorTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        private static void VerifyVisitor(ProtoDescriptor descriptor, VisitorVerificationAction verify = null)
        {
            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
            var visitor = new CodeGenerationProtoDescriptorVisitor { };
            visitor.Visit(descriptor.AssertNotNull());
            verify?.Invoke(visitor, descriptor);
        }

        // TODO: TBD: where were we headed with these? apparently somewhere else why were they private?
        private static void VerifyVisitorThrows<TException>(VisitorVerificationAction verifyVisitor
            , ProtoDescriptor descriptor, Action<TException> verifyException = null)
            where TException : Exception
        {
            Action GetVisitorVerificationAction() => () => VerifyVisitor(descriptor, verifyVisitor);
            var exception = GetVisitorVerificationAction().AssertThrows<TException>();
            verifyException?.Invoke(exception);
        }

        [Fact(Skip = "Come back to this one later, pending we determine what the intention was here, what were we verifying...")]
        public void Visit_Without_Package_Statement_Throws()
        {
            // TODO: TBD: As the method name suggests?
        }
    }
}
