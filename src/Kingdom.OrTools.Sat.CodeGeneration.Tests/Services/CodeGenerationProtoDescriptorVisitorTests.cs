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

            Assert.NotNull(descriptor);

            visitor.Visit(descriptor);

            verify?.Invoke(visitor, descriptor);
        }

        private static void VerifyVisitorThrows<TException>(VisitorVerificationAction verifyVisitor
            , ProtoDescriptor descriptor, Action<TException> verifyException = null)
            where TException : Exception
        {
            var exception = Assert.Throws<TException>(() => VerifyVisitor(descriptor, verifyVisitor));
            verifyException?.Invoke(exception);
        }

        [Fact]
        public void Visit_Without_Package_Statement_Throws()
        {
        }
    }
}
