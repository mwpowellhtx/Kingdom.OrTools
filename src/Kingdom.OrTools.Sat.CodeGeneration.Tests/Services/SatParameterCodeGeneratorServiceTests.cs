namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Xunit.Abstractions;

    public class SatParameterCodeGeneratorServiceTests
        : SatParameterCodeGeneratorServiceTestFixtureBase<SatParameterCodeGeneratorService>
    {
        public SatParameterCodeGeneratorServiceTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }
    }
}
