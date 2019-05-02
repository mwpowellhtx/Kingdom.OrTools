namespace Kingdom.OrTools.Sat.CodeGeneration
{
    public class SatParameterCodeGeneratorServiceFixture : SatParameterCodeGeneratorService
    {
        private const string FixtureResourcePath = "Resources.#sat_parameters.proto";

        public SatParameterCodeGeneratorServiceFixture()
            : base(FixtureResourcePath)
        {
        }
    }
}
