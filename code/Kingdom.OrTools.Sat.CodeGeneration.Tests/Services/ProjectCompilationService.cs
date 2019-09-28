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
    using static Ranges;

    public class ProjectCompilationService
    {
        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        internal static ProjectCompilationService Instance => new ProjectCompilationService { };

        /// <summary>
        /// Private Constructor.
        /// </summary>
        private ProjectCompilationService()
        {
        }

        private ITestOutputHelper _outputHelper;

        /// <summary>
        /// Gets or Sets the OutputHelper.
        /// </summary>
        internal ITestOutputHelper OutputHelper
        {
            get => _outputHelper;
            set => _outputHelper = value.AssertNotNull();
        }

        internal static MetadataReference GetMetadataReference<T>() => MetadataReference.CreateFromFile(
            typeof(T).Assembly.Location
        );

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

        internal virtual void Verify(FileInfo projectPathInfo, OutputKind outputKind, MetadataReferencesFactory metadataReferencesFactory)
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
                
                if (metadataReferences.AssertNotNull().Any())
                {
                    project = project.AddMetadataReferences(metadataReferences).AssertNotNull();
                }

                var compilation = project.GetCompilationAsync().Result.AssertNotNull();

                using (var stream = new MemoryStream())
                {
                    var result = compilation.Emit(stream).AssertNotNull();
                    result.Diagnostics.ToList().ForEach(Report);
                    result.AssertTrue(x => x.Success);
                }
            }
        }
    }
}
