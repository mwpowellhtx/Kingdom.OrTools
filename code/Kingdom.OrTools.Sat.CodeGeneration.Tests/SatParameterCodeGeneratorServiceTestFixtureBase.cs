namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Xunit;
    using Xunit.Abstractions;

    public abstract class SatParameterCodeGeneratorServiceTestFixtureBase<TService> : TestFixtureBase
        where TService : SatParameterCodeGeneratorServiceBase
    {
        /// <summary>
        /// Gets an Instance of the <typeparamref name="TService"/>.
        /// </summary>
        protected abstract TService Service { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected SatParameterCodeGeneratorServiceTestFixtureBase(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public void Service_Is_NotNull() => Service.AssertNotNull();

        [Fact]
        public void Service_IsType_Is_Correct() => Service.AssertNotNull().AssertIsType<TService>();

        [Fact]
        public void Service_ServiceType_Is_Correct() => Service.AssertNotNull().ServiceType.AssertEqual(typeof(TService));

        [Fact]
        public void Service_ServiceType_Is_Aligned() => Service.AssertNotNull().AssertIsType(Service.ServiceType.AssertNotNull());

        /// <summary>
        /// Verifies that the <see cref="SatParameterCodeGeneratorService.ResourcePath"/> Exists.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns>The <paramref name="resourcePath"/> after an initial assertion pass.</returns>
        protected virtual string VerifyResourceExists(string resourcePath) => resourcePath.AssertNotNull().AssertNotEmpty();

        [Fact]
        public void ResourcePath_ResourceExists() => VerifyResourceExists(Service.AssertNotNull().ResourcePath);

        [Fact]
        public void SatParameters_ProtocolBuffers_Stream_NotNull() => Service.AssertNotNull().SatParametersProtocolBufferStream.AssertNotNull().Dispose();
    }
}
