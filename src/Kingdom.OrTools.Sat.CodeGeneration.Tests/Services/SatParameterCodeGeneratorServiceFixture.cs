using System;
using System.IO;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using static String;

    // TODO: TBD: how much of this project output is test fixture stuff?
    // TODO: TBD: I am thinking this is an approach that needs to be reverted and just output the CompilationUnitSyntax itself...
    // TODO: TBD: saving project folders? project file? making a project reference? for internal verification, we can do that...
    // TODO: TBD: delivering into a Code.Generation.Roslyn context, all we need is are the CompilationUnitSyntax...
    public class SatParameterCodeGeneratorServiceFixture : InternalSatParameterCodeGeneratorService
    {
        private static Guid NewId => Guid.NewGuid();

        private string GeneratorOutputDirectory { get; } = $"{NewId:N}";

        public event EventHandler<FileEventArgs> CodeGenerated;

        // TODO: TBD: however, just behind identifying a correct Protocol Buffer stream and evaluating the parser visitor...
        // TODO: TBD: comes the question what to do with that, we just want to generate CompilationUnitSyntax...
        // TODO: TBD: files and projects and so forth are just here as a unit test requirement...
        // TODO: TBD: the difficulty we will have here are the layers and potentially moving parts informing this dynamic...
        // TODO: TBD: perhaps will not be that terrible to re-factor said bits into the unit test service fixture?
        // TODO: TBD: which also apparently had its own issues with correctly pinned resource versions, replete with `.proto´ bug fixes in and of itself...
        private void OnCodeGenerated(FileInfo info, string text) => CodeGenerated?.Invoke(this, new FileEventArgs(info, text));

        private void GenerateCode(string path, Func<string> generator)
        {
            if (File.Exists(path))
            {
                return;
            }

            var text = generator();
            using (var stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(text);
                }
            }

            OnCodeGenerated(new FileInfo(path), text);
        }

        private void GenerateServiceProjectFile(ref FileSystemInfo resultInfo)
        {
            if (!GenerateProjectBits)
            {
                return;
            }

            const string crLf = "\r\n";

            var dirBuildPropsPath = $"{GeneratorOutputDirectory}\\Directory.Build.props";
            var projectPath = $"{GeneratorOutputDirectory}\\{NewId:N}.csproj";

            resultInfo = new FileInfo(projectPath);

            if (!File.Exists(dirBuildPropsPath))
            {
                // Do this to avoid invoking any unnecessary or extraneous bits during the build.
                GenerateCode(dirBuildPropsPath, () => Join(crLf, "<Project>", "</Project>"));
            }

            if (!File.Exists(projectPath))
            {
                const string parametersCoreProjectName = "Kingdom.OrTools.Sat.Parameters";

                // Which paths are based on the output directory relative paths.
                GenerateCode(projectPath, () => Join(crLf
                    , @"<Project Sdk=""Microsoft.NET.Sdk"">"
                    , "  <PropertyGroup>"
                    , "    <TargetFramework>netstandard2.0</TargetFramework>"
                    , "  </PropertyGroup>"
                    , "  <ItemGroup>"
                    , $@"    <ProjectReference Include=""..\..\..\..\..\{parametersCoreProjectName}\{parametersCoreProjectName}.csproj"" />"
                    , "  </ItemGroup>"
                    , "</Project>")
                );
            }
        }

        /// <summary>
        /// Generates the Code. Returns the CSharp Project <see cref="FileInfo"/> in a test
        /// context. Returns the corresponding <see cref="DirectoryInfo"/> where the code
        /// landed in a production context.
        /// </summary>
        /// <returns></returns>
        internal FileSystemInfo GenerateCode()
        {
            var descriptor = Descriptor;

            // TODO: TBD: CG, so-called, in the compilation verification sense of the term, is based entirely on the CG Visitor.
            var visitor = CodeGenerationVisitor;

            visitor.Visit(descriptor);

            FileSystemInfo resultInfo = new DirectoryInfo(GeneratorOutputDirectory);

            // ReSharper disable once InvertIf
            if (!resultInfo.Exists)
            {
                Directory.CreateDirectory(GeneratorOutputDirectory);

                GenerateServiceProjectFile(ref resultInfo);

                // TODO: TBD: Which, here is the key, sort of...
                // TODO: TBD: I am also wondering if possibly the CGR ought not to expose its CG internals as a dictionary along these lines...
                foreach (var x in visitor.CompilationUnits)
                {
                    string RenderCompilationUnit() => $"{x.Value.GetText()}";
                    GenerateCode($"{GeneratorOutputDirectory}\\{NewId:N}.cs", RenderCompilationUnit);
                }
            }

            return resultInfo;
        }
    }
}
