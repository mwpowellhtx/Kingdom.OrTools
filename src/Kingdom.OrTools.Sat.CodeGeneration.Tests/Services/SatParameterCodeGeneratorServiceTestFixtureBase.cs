using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Xunit;
    using Xunit.Abstractions;
    using static Microsoft.CodeAnalysis.OutputKind;
    using static String;

    public abstract class SatParameterCodeGeneratorServiceTestFixtureBase<
            TService> : CodeGenerationVerificationTestFixtureBase
        where TService : SatParameterCodeGeneratorService, new()
    {
        protected SatParameterCodeGeneratorServiceTestFixtureBase(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies the <see cref="SatParameterCodeGeneratorService"/>, sets the
        /// <see cref="SatParameterCodeGeneratorService.GenerateProjectBits"/> to True
        /// for test purposes.
        /// </summary>
        /// <param name="verify"></param>
        protected virtual void VerifyService(Action<SatParameterCodeGeneratorService> verify = null)
        {
            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
            var service = new TService { };

            service.GenerateProjectBits = true;

            verify?.Invoke(service);
        }

        [Fact]
        public void ServiceType_Is_Correct()
        {
            VerifyService(service =>
            {
                Assert.IsType(service.ServiceType.AssertNotNull(), service);
                OutputHelper.WriteLine($"{nameof(service.ServiceType)} = '{service.ServiceType.FullName}'");
            });
        }

        protected virtual IEnumerable<string> ResourcePathElements
        {
            get
            {
                yield return "Resources";
                yield return "sat_parameters";
                yield return "proto";
            }
        }

        [Fact]
        public virtual void ResourcePath_Is_Correct()
        {
            VerifyService(service =>
            {
                var resourcePath = service.ResourcePath;
                Assert.Equal(ResourcePathElements, resourcePath.Split('.'));
                OutputHelper.WriteLine($"{nameof(service.ResourcePath)} = '{resourcePath}'");
            });
        }

        [Fact]
        public void Stream_Is_Correct()
        {
            VerifyService(service =>
            {
                var stream = service.SatParametersProtocolBufferStream.AssertNotNull();
                using (stream)
                {
                    stream.AssertTrue(x => x.CanRead)
                        .AssertTrue(x => x.CanSeek)
                        .AssertFalse(x => x.CanWrite);

                    Assert.Equal(0L, stream.Position);
                    OutputHelper.WriteLine($"Stream Length = {stream.Length}");
                }
            });
        }

        [Fact]
        public void Source_Is_Correct()
        {
            VerifyService(service =>
            {
                long expectedLength;

                using (var stream = service.SatParametersProtocolBufferStream)
                {
                    expectedLength = stream.Length;
                }

                var source = service.SatParametersProtocolBufferSource;

                Assert.NotEmpty(source.AssertNotNull());

                Assert.Equal(expectedLength, source.Length);

                OutputHelper.WriteLine($"<{nameof(source)}>");
                OutputHelper.WriteLine($"{source}");
                OutputHelper.WriteLine($"</{nameof(source)}>");
            });
        }

        [Fact]
        public void Descriptor_Parses() => VerifyService(service => service.Descriptor.AssertNotNull());

        [Fact]
        public void Compilation_Units_Are_Generated()
        {
            VerifyService(service =>
            {
                var descriptor = service.Descriptor;

                descriptor.AssertNotNull();

                service.CodeGenerationVisitor.Visit(descriptor);

                var compilationUnits = service.CodeGenerationVisitor.CompilationUnits;

                Assert.NotEmpty(compilationUnits.AssertNotNull());

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
        }

        [Fact]
        public void Code_Can_Be_Generated_Without_Project_Bits()
        {
            VerifyService(service =>
            {
                const string crLf = "\r\n";
                service.GenerateProjectBits = false;

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

                service.CodeGenerated += (sender, e) => Report(e.Info, e.Text);

                var generatedCodeDirInfo = service.GenerateCode()
                    .AssertIsAssignableFrom<DirectoryInfo>()
                    .AssertTrue(x => Directory.Exists(x.FullName));

                const string generated = nameof(generated);

                generatedPaths.AssertNotNull(x => OutputHelper.WriteLine(Join(crLf
                            , $"<{generated}>"
                            , $"  <SourcePaths count=\"{x.Count}\" />"
                            , $"</{generated}>"
                        ))
                    )
                    .AssertAll(x => x.AssertTrue(_ => !IsNullOrEmpty(x) && x.EndsWith(".cs")));

                const bool recursive = true;

                // Null will have already been established.
                generatedCodeDirInfo.Delete(recursive);
            });
        }

        [Fact]
        public void Code_Can_Be_Generated_With_Project_Bits()
        {
            VerifyService(service =>
            {
                // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                var generatedPaths = new List<string> { };

                void Report(FileInfo info, string text)
                {
                    const string crLf = "\r\n";

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

                service.CodeGenerated += (sender, e) => Report(e.Info, e.Text);

                var generatedProjectFileInfo = service.GenerateCode()
                    .AssertIsAssignableFrom<FileInfo>()
                    .AssertTrue(x => x.Exists);

                foreach (var ext in GetRange(".props", ".csproj"))
                {
                    generatedPaths.SingleOrDefault(x => !IsNullOrEmpty(x) && x.EndsWith(ext)).AssertNotNull();
                }

                Assert.Equal(generatedPaths.Count - 2
                    , generatedPaths.Count(x => !IsNullOrEmpty(x) && x.EndsWith(".cs")));

                VerifyProjectCompilation(generatedProjectFileInfo, DynamicallyLinkedLibrary
                    , () => GetRange(
                        GetMetadataReference<object>()
                    )
                );

                const bool recursive = true;

                generatedProjectFileInfo.Directory.AssertNotNull(x => x.Exists.AssertTrue()).Delete(recursive);
            });
        }
    }
}
