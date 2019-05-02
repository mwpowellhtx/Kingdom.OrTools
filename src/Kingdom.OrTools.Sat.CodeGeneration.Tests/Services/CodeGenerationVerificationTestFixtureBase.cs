using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.MSBuild;
    using Xunit;
    using Xunit.Abstractions;
    using static Microsoft.CodeAnalysis.LocationKind;

    public abstract class CodeGenerationVerificationTestFixtureBase : TestFixtureBase
    {
        protected CodeGenerationVerificationTestFixtureBase(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        protected static MetadataReference GetMetadataReference<T>() => MetadataReference.CreateFromFile(
            typeof(T).Assembly.Location
        );

        protected delegate IEnumerable<MetadataReference> MetadataReferencesFactory();

        private static void Report(Diagnostic diagnostic, ITestOutputHelper outputHelper)
        {
            string GetLocationReport() => GetRange(SourceFile, ExternalFile, XmlFile)
                .Contains(diagnostic.Location.Kind)
                ? $"{diagnostic.Location}: "
                : string.Empty;

            outputHelper.WriteLine($"{GetLocationReport()}{diagnostic.GetMessage()}");
            outputHelper.WriteLine(string.Empty);
        }

        private void Report(Diagnostic diagnostic) => Report(diagnostic, OutputHelper);

        protected virtual void VerifyProjectCompilation(FileInfo projectPathInfo
            , OutputKind outputKind, MetadataReferencesFactory metadataReferencesFactory)
        {
            projectPathInfo.Exists.AssertTrue();

            using (var workspace = MSBuildWorkspace.Create().AssertNotNull())
            {
                var project = workspace.OpenProjectAsync(projectPathInfo.FullName).Result.AssertNotNull();

                {
                    var options = new CSharpCompilationOptions(outputKind);
                    project = project.WithCompilationOptions(options).AssertNotNull();
                }

                var metadataReferences = metadataReferencesFactory().Select(x => x.AssertNotNull()).ToArray();
                Assert.NotNull(metadataReferences);

                if (metadataReferences.Any())
                {
                    project = project.AddMetadataReferences(metadataReferences).AssertNotNull();
                }

                var compilation = project.GetCompilationAsync().Result.AssertNotNull();

                using (var stream = new MemoryStream())
                {
                    var result = compilation.Emit(stream).AssertNotNull();
                    result.Diagnostics.ToList().ForEach(Report);
                    Assert.True(result.Success);
                }
            }
        }
    }
}
