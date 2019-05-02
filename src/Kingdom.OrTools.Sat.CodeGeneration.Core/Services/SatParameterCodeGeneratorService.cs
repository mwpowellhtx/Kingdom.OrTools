using System;
using System.IO;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Antlr4.Runtime;
    using Microsoft.CodeAnalysis;
    using Protobuf;
    using static String;
    using ProtoDeclContext = Protobuf.ProtoParser.ProtoDeclContext;

    // TODO: TBD: bridge from the git gotten or-tools
    public class SatParameterCodeGeneratorService
    {
        /// <summary>
        /// Gets or sets whether to Generate Project Bits. Default is False.
        /// </summary>
        internal bool GenerateProjectBits { get; set; } = false;

        internal string GeneratorOutputDirectory { get; set; } = $"{NewId:N}";

        internal const string DefaultResourcePath = @"Resources.sat_parameters.proto";

        public SatParameterCodeGeneratorService()
            : this(DefaultResourcePath)
        {
        }

        public SatParameterCodeGeneratorService(string resourcePath)
        {
            ResourcePath = resourcePath;
        }

        private CodeGenerationProtoDescriptorVisitor _codeGenerationVisitor;

        internal CodeGenerationProtoDescriptorVisitor CodeGenerationVisitor
            => _codeGenerationVisitor ?? (_codeGenerationVisitor
                   // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                   = new CodeGenerationProtoDescriptorVisitor { });

        private Type _serviceType;

        internal Type ServiceType => _serviceType ?? (_serviceType = GetType());

        internal string ResourcePath { get; }


        internal Stream SatParametersProtocolBufferStream
            => ServiceType.Assembly.GetManifestResourceStream(ServiceType, ResourcePath);

        // ReSharper disable once CommentTypo
        // TODO: TBD: the stop-gap measure is to xcopy the proto source files from a clone of the Google.OrTools repository.
        // TODO: TBD: this approach is adequate to get started with, but eventually we want for the dependency to run seamlessly via NuGet packaging.
        /// <summary>
        /// Gets the Sat Parameters Protocol Buffer source code.
        /// </summary>
        /// <remarks>This works because we carry a development only dependency to Google.OrTools,
        /// which we want to include pre-packaged .proto files. Whichever means we employ in order
        /// to update the source dependency, we will bake it in as a resource, from which we can
        /// extract the source code from the Assembly Manifest.</remarks>
        /// <see cref="!:http://github.com/google/or-tools/issues/1190">Package the .proto files with the NuGet package</see>
        /// <see cref="!:http://github.com/google/or-tools"/>
        /// <see cref="!:http://www.nuget.org/packages/Google.OrTools"/>
        internal string SatParametersProtocolBufferSource
        {
            get
            {
                using (var stream = SatParametersProtocolBufferStream)
                {
                    if (stream == null)
                    {
                        throw new InvalidOperationException($"Resource '{ResourcePath}' not found.");
                    }

                    using (var sr = new StreamReader(stream))
                    {
                        return sr.ReadToEndAsync().Result;
                    }
                }
            }
        }

        private ProtoDescriptor _descriptor;

        /// <summary>
        /// Gets the Descriptor based on the <see cref="SatParametersProtocolBufferSource"/>.
        /// First and foremost, before the service can do any code generation, the resource
        /// must be able to be loaded and obtained from <see cref="Stream"/> into
        /// <see cref="string"/> and then Parsed to <see cref="ProtoDescriptor"/>. Then
        /// we may visit the Descriptor and generate the bits that we need to. We may even
        /// require several visitors for various cross-cutting aspects.
        /// </summary>
        internal ProtoDescriptor Descriptor
        {
            get
            {
                ProtoDeclContext EvaluateCallback(ProtoParser parser) => parser.protoDecl();

                ProtoDescriptor EvaluateProtoDescriptor()
                {
                    var source = SatParametersProtocolBufferSource;

                    var listener = source.Trim().WalkEvaluatedContext<ProtoLexer, CommonTokenStream, ProtoParser
                        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                        , ProtoDeclContext, ProtoDescriptorListener>(EvaluateCallback, new DefaultErrorListener { });

                    return listener.ActualProto;
                }

                return _descriptor ?? (_descriptor = EvaluateProtoDescriptor());
            }
        }

        private static Guid NewId => Guid.NewGuid();

        public event EventHandler<FileEventArgs> CodeGenerated;

        private void OnCodeGenerated(FileInfo info, string text)
            => CodeGenerated?.Invoke(this, new FileEventArgs(info, text));

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

        /// <summary>
        /// Generates the Code. Returns the CSharp Project <see cref="FileInfo"/> in a test
        /// context. Returns the corresponding <see cref="DirectoryInfo"/> where the code
        /// landed in a production context.
        /// </summary>
        /// <returns></returns>
        public FileSystemInfo GenerateCode()
        {
            var descriptor = Descriptor;

            var visitor = CodeGenerationVisitor;

            visitor.Visit(descriptor);

            FileSystemInfo resultInfo = new DirectoryInfo(GeneratorOutputDirectory);

            // ReSharper disable once InvertIf
            if (!resultInfo.Exists)
            {
                const string crLf = "\r\n";

                Directory.CreateDirectory(GeneratorOutputDirectory);

                if (GenerateProjectBits)
                {
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
                        const string parametersCoreProjectName = "Kingdom.OrTools.Sat.Parameters.Core";

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
