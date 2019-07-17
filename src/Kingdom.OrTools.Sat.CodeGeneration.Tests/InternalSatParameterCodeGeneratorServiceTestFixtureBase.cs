using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Xunit;
    using Xunit.Abstractions;
    using static Microsoft.CodeAnalysis.OutputKind;
    using static ProjectCompilationService;
    using static Ranges;
    using static String;

    // TODO: TBD: some of this needs to be deferred from the base class even from here, we think...
    // TODO: TBD: establish an intermediate base class that simply verifies the abstract SatParameterCodeGeneratorServiceBase aspects...
    // TODO: TBD: this would also be leveraged for purposes of verifying what we think is the production service implementation, ...
    // TODO: TBD: that is, which is dependent on the most up to date Google.OrTools package reference...
    public abstract class InternalSatParameterCodeGeneratorServiceTestFixtureBase<
            TService> : SatParameterCodeGeneratorServiceTestFixtureBase<TService>
        where TService : InternalSatParameterCodeGeneratorService, new()
    {
        /// <inheritdoc />
        protected override TService Service => new TService {GenerateProjectBits = true};

        private ProjectCompilationService CompilationService { get; }

        protected InternalSatParameterCodeGeneratorServiceTestFixtureBase(ITestOutputHelper outputHelper, ProjectCompilationService compilationService)
            : base(outputHelper)
        {
            CompilationService = compilationService.AssertNotNull().Initialize(x => x.OutputHelper = outputHelper);
        }

        /// <summary>
        /// Verifies the <see cref="InternalSatParameterCodeGeneratorService"/>, sets the
        /// <see cref="InternalSatParameterCodeGeneratorService.GenerateProjectBits"/> to
        /// True for test purposes.
        /// </summary>
        /// <param name="verify"></param>
        protected virtual void VerifyService(Action<InternalSatParameterCodeGeneratorService> verify = null) => verify?.Invoke(Service.AssertNotNull());

        [Fact]
        public void ServiceType_Is_Correct() => VerifyService(service =>
        {
            service.AssertIsType(service.ServiceType.AssertNotNull());
            OutputHelper.WriteLine($"{nameof(service.ServiceType)} = '{service.ServiceType.FullName}'");
        });

        [Fact]
        public virtual void ResourcePath_Is_Correct() => VerifyService(service =>
        {
            IEnumerable<string> GetResourcePathElements()
            {
                yield return "Resources";
                yield return "sat_parameters";
                yield return "proto";
            }

            var resourcePath = service.ResourcePath;
            resourcePath.Split('.').AssertEqual(GetResourcePathElements());
            OutputHelper.WriteLine($"{nameof(service.ResourcePath)} = '{resourcePath}'");
        });

        [Fact]
        public void Stream_Is_Correct() => VerifyService(service =>
        {
            long streamLength;

            using (var stream = service.SatParametersProtocolBufferStream.AssertNotNull())
            {
                stream.AssertTrue(x => x.CanRead)
                    .AssertTrue(x => x.CanSeek)
                    .AssertFalse(x => x.CanWrite);

                stream.Position.AssertEqual(0L);

                streamLength = stream.Length;
            }

            OutputHelper.WriteLine($"Stream Length = {streamLength}");
        });

        [Fact]
        public void Source_Is_Correct() => VerifyService(service =>
        {
            long expectedLength;

            using (var stream = service.SatParametersProtocolBufferStream)
            {
                expectedLength = stream.Length;
            }

            var source = service.SatParametersProtocolBufferSource;

            source.AssertNotNull().AssertNotEmpty();
            ((long) source.Length).AssertEqual(expectedLength);

            OutputHelper.WriteLine($"<{nameof(source)}>");
            OutputHelper.WriteLine($"{source}");
            OutputHelper.WriteLine($"</{nameof(source)}>");
        });

        [Fact]
        public void Descriptor_Parses() => VerifyService(service => service.Descriptor.AssertNotNull());

        [Fact]
        public void Compilation_Units_Are_Generated() => VerifyService(service =>
        {
            var descriptor = service.Descriptor;

            descriptor.AssertNotNull();

            service.CodeGenerationVisitor.Visit(descriptor);

            var compilationUnits = service.CodeGenerationVisitor.CompilationUnits;

            compilationUnits.AssertNotNull().AssertNotEmpty();

            const string generated = nameof(generated);

            foreach (var (key, code) in compilationUnits)
            {
                OutputHelper.WriteLine($"<{generated}>");
                OutputHelper.WriteLine($"  <{nameof(key)}>{key:D}</{nameof(key)}>");
                OutputHelper.WriteLine($"  <{nameof(code)}>");
                // GetText v ToString, which includes any Trivia.
                OutputHelper.WriteLine($"{code.GetText()}");
                OutputHelper.WriteLine($"  </{nameof(code)}>");
                OutputHelper.WriteLine($"</{generated}>");
            }
        });

        [Fact]
        public void Code_Can_Be_Generated_Without_Project_Bits() => VerifyService(s =>
        {
            const string crLf = "\r\n";
            s.GenerateProjectBits = false;

            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
            var generatedPaths = new List<string> { };

            void Report(FileInfo info, string text)
            {
                var exists = info.Exists.AssertTrue();
                var lengthBytes = info.Length.AssertTrue(x => x > 0L);
                var path = info.FullName.AssertTrue(File.Exists);

                generatedPaths.Add(info.FullName);

                OutputHelper.WriteLine(Join(crLf, $"<{nameof(info)}>"
                    , $"    <{nameof(exists)}>{exists}</{nameof(exists)}>"
                    , $"    <{nameof(lengthBytes)}>{lengthBytes}</{nameof(lengthBytes)}>"
                    , $"    <{nameof(path)}>{path}</{nameof(path)}>"
                    , $"</{nameof(info)}>")
                );

                OutputHelper.WriteLine(Join(crLf
                    , $"<{nameof(text)}>"
                    , text
                    , $"</{nameof(text)}>")
                );
            }

            var service = s.AssertIsAssignableFrom<SatParameterCodeGeneratorServiceFixture>();

            service.CodeGenerated += (sender, e) => Report(e.Info, e.Text);

            var generatedCodeDirInfo = service.GenerateCode()
                .AssertIsAssignableFrom<DirectoryInfo>()
                .AssertTrue(x => Directory.Exists(x.FullName));

            const string generated = nameof(generated);

            // Asserting All, paths not null, not empty, ending with.
            generatedPaths.AssertNotNull(x => OutputHelper.WriteLine(Join(crLf
                        , $"<{generated}>"
                        , $"  <SourcePaths count=\"{x.Count}\" />"
                        , $"</{generated}>"
                    ))
                )
                .AssertAll(x => x.AssertTrue(
                    _ => x.AssertNotNull().AssertNotEmpty().EndsWith(".cs")
                ));

            const bool recursive = true;

            // Null will have already been established.
            generatedCodeDirInfo.Delete(recursive);
        });

        [Fact]
        public void Code_Can_Be_Generated_With_Project_Bits() => VerifyService(s =>
        {
            const string crLf = "\r\n";

            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
            var generatedPaths = new List<string> { };

            void Report(FileInfo info, string text)
            {
                var exists = info.Exists.AssertTrue();
                var lengthBytes = info.Length.AssertTrue(x => x > 0L);
                var path = info.FullName.AssertTrue(File.Exists);

                generatedPaths.Add(info.FullName);

                OutputHelper.WriteLine(Join(crLf, $"<{nameof(info)}>"
                    , $"    <{nameof(exists)}>{exists}</{nameof(exists)}>"
                    , $"    <{nameof(lengthBytes)}>{lengthBytes}</{nameof(lengthBytes)}>"
                    , $"    <{nameof(path)}>{path}</{nameof(path)}>"
                    , $"</{nameof(info)}>")
                );

                OutputHelper.WriteLine(Join(crLf
                    , $"<{nameof(text)}>"
                    , text
                    , $"</{nameof(text)}>")
                );
            }

            var service = s.AssertIsAssignableFrom<SatParameterCodeGeneratorServiceFixture>();

            service.CodeGenerated += (sender, e) => Report(e.Info, e.Text);

            var generatedProjectFileInfo = service.GenerateCode()
                .AssertIsAssignableFrom<FileInfo>()
                .AssertTrue(x => x.Exists);

            foreach (var ext in GetRange(".props", ".csproj"))
            {
                // Same bits here concerning path verification, not null, not empty.
                generatedPaths.SingleOrDefault(x => x.AssertNotNull().AssertNotEmpty().EndsWith(ext)).AssertNotNull();
            }

            generatedPaths.Count(x => !IsNullOrEmpty(x) && x.EndsWith(".cs"))
                .AssertEqual(generatedPaths.Count - 2)
                ;

            CompilationService.Verify(generatedProjectFileInfo, DynamicallyLinkedLibrary
                , () => GetRange(
                    GetMetadataReference<object>()
                )
            );

            const bool recursive = true;

            generatedProjectFileInfo.Directory.AssertNotNull(x => x.Exists.AssertTrue()).Delete(recursive);
        });
    }
}
