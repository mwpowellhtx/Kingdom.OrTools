namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Xunit.Abstractions;

    public class InternalSatParameterCodeGeneratorServiceTests
        : InternalSatParameterCodeGeneratorServiceTestFixtureBase<
            SatParameterCodeGeneratorServiceFixture>
    {
        /// <summary>
        /// Default Constructor. We could probably let XUnit feed the
        /// <see cref="ProjectCompilationService"/> instance, but this will guarantee
        /// that we provide an expected instance.
        /// </summary>
        /// <param name="outputHelper"></param>
        public InternalSatParameterCodeGeneratorServiceTests(ITestOutputHelper outputHelper)
            : base(outputHelper, ProjectCompilationService.Instance)
        {
        }
    }
}
