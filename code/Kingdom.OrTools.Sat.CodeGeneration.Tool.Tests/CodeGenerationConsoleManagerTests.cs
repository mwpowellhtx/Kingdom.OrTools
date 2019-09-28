using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NConsole.Options;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Code.Generation.Roslyn;
    using Xunit;
    using Xunit.Abstractions;
    using static ConsoleManager;
    using static CodeGenerationConsoleManager;
    using static CodeGenerationConsoleManager.Constants;
    using static CodeGenerationConsoleManagerTests.Constants;
    using static String;

    /// <summary>
    /// We ought to be able to integration test everything behind the `dotnet tool-name´
    /// up to this point. That should give us a much higher level of confidence that everything
    /// we thing should be working is in fact working when it comes time to actually connect the
    /// tooling dots.
    /// </summary>
    public class CodeGenerationConsoleManagerTests : TestFixtureBase
    {
        private CodeGenerationConsoleManager Manager { get; }

        private string OutputDirectory { get; }

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        private ICollection<string> Arguments { get; } = new List<string> { };

        private StringBuilder[] StringBuilders { get; }

        private StringWriter[] StringWriters { get; }

        public CodeGenerationConsoleManagerTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            OutputDirectory = $"{Guid.NewGuid():D}";

            // First is the OutputWriter, second is the ErrorWriter.
            StringBuilders = GetRange(new StringBuilder(), new StringBuilder()).ToArray();
            StringWriters = StringBuilders.Select(sb => new StringWriter(sb)).ToArray();

            //was using: new TestOutputHelperTextWriter(OutputHelper)
            Manager = new CodeGenerationConsoleManager(StringWriters[0], StringWriters[1]);
        }

        internal delegate void ConfigureManagerCallback(CodeGenerationConsoleManager manager, ICollection<string> arguments);

        // ReSharper disable InconsistentNaming
        internal static class Constants
        {
            /// <summary>
            /// &quot;--out-dir&quot;
            /// </summary>
            internal const string out_dir = "--out-dir";

            /// <summary>
            /// &quot;--registry-file&quot;
            /// </summary>
            internal const string registry_file = "--registry-file";

            /// <summary>
            /// &apos;.&apos;
            /// </summary>
            private const char dot = '.';

            /// <summary>
            /// &quot;.json&quot;
            /// </summary>
            /// <see cref="dot"/>
            internal static readonly string json = $"{dot}{nameof(json)}";

            private static string _registryFileName;

            internal static string RegistryFileName
                => _registryFileName ?? (_registryFileName
                       = $"{typeof(Constants).Namespace}.Tool.Tests{g}{json}"
                   );
        }
        // ReSharper restore InconsistentNaming

        private void ConfigureDefaultArguments(ICollection<string> args)
        {
            args.AssertSame(Arguments);
            args.AddRange(out_dir, OutputDirectory, registry_file, RegistryFileName).AssertSame(Arguments);
        }

        internal void VerifyManager(ConfigureManagerCallback configure, int expectedErrorLevel
            , out CodeGenerationConsoleManager manager, out OrToolsSatGeneratedSyntaxTreeRegistry registry)
        {
            manager = Manager.AssertNotNull();

            configure.AssertNotNull().Invoke(manager, Arguments);

            manager.TryParseOrShowHelp(Arguments.ToArray()).AssertTrue();

            manager.Run(out var actualErrorLevel);

            // Whatever the outcome was, obtain that Registry.
            registry = manager.Registry;

            actualErrorLevel.AssertEqual(expectedErrorLevel);

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (expectedErrorLevel)
            {
                case DefaultErrorLevel:
                    registry.AssertNotNull().AssertNotEmpty();
                    break;

                case ErrorGeneratingCode:
                    // TODO: TBD: may elaborate on what this means, `error´ ...
                    break;

                case MustSpecifyOutputDirectory:
                    manager.OutputDirectoryVar.Value.AssertTrue(IsNullOrEmpty);
                    registry.AssertNull();
                    break;

                case MustSpecifyRegistryFileName:
                    manager.OutputDirectoryVar.Value.AssertNotNull().AssertNotEmpty();
                    manager.GeneratedCodeRegistryFileVar.Value.AssertTrue(IsNullOrEmpty);
                    registry.AssertNull();
                    break;
            }
        }

        internal void VerifyManager(ConfigureManagerCallback configure, int expectedErrorLevel)
            => VerifyManager(configure, expectedErrorLevel, out _, out _);

        internal TException VerifyManagerThrows<TException>(ConfigureManagerCallback configure)
            where TException : Exception
        {
            var manager = Manager;
            configure.AssertNotNull().Invoke(manager, Arguments);

            Action run = () =>
            {
                manager.TryParseOrShowHelp(Arguments.ToArray()).AssertTrue();
                manager.Run(out _);
            };

            return run.AssertThrows<TException>();
        }

        [Fact]
        public void Unspecified_OutputDirectory_Yields_Error() => VerifyManager(
            (m, a) => a.AddRange(registry_file, RegistryFileName).AssertSame(Arguments)
            , MustSpecifyOutputDirectory
        );

        [Fact]
        public void Unspecified_RegistryFileName_Yields_Error() => VerifyManager(
            (m, a) => a.AddRange(out_dir, OutputDirectory).AssertSame(Arguments)
            , MustSpecifyRegistryFileName
        );

        private void VerifySingleCodeGenerationPass(out CodeGenerationConsoleManager manager
            , out OrToolsSatGeneratedSyntaxTreeRegistry registry)
        {
            // TODO: TBD: for a single pass, it might make sense to refactor this block of code... rinse and repeat for other scenarios...
            VerifyManager((m, a) => ConfigureDefaultArguments(a), DefaultErrorLevel, out manager, out registry);

            var local = registry;

            Path.Combine(registry.OutputDirectory, RegistryFileName).AssertFileExists();

            void VerifyGeneratedFileExists(Guid key) => Path.Combine(local.OutputDirectory, key.RenderGeneratedFileName()).AssertFileExists();

            registry.AssertNotNull().AssertNotEmpty().SelectMany(x => x.GeneratedAssetKeys)
                .ToList().ForEach(VerifyGeneratedFileExists);
        }

        private void VerifySingleCodeGenerationPass(out OrToolsSatGeneratedSyntaxTreeRegistry registry)
            => VerifySingleCodeGenerationPass(out _, out registry);

        [Fact]
        public void Single_Code_Generation_Pass_Correct() => VerifySingleCodeGenerationPass(out _);

        [Fact]
        public void No_Op_Second_Code_Generation_Attempt()
        {
            VerifySingleCodeGenerationPass(out var registry);

            VerifySingleCodeGenerationPass(out var secondRegistry);

            registry.AssertNotNull().AssertNotEmpty().AssertNotSame(
                secondRegistry.AssertNotNull().AssertNotEmpty()
            );

            registry.SelectMany(x => x.GeneratedAssetKeys).AssertEqual(
                secondRegistry.SelectMany(x => x.GeneratedAssetKeys)
            );
        }

        private static void VerifyDifferentRegistries(OrToolsSatGeneratedSyntaxTreeRegistry previousRegistry
            , OrToolsSatGeneratedSyntaxTreeRegistry currentRegistry)
        {
            {
                bool DirectoryExists(OrToolsSatGeneratedSyntaxTreeRegistry x) => Directory.Exists(x.OutputDirectory);

                // Do some quick verification of each Registry.
                previousRegistry = previousRegistry.AssertNotNull().AssertNotEmpty().AssertTrue(DirectoryExists);
                currentRegistry = currentRegistry.AssertNotNull().AssertNotEmpty().AssertTrue(DirectoryExists);
            }

            // Should always Not be the Same.
            previousRegistry.AssertNotSame(currentRegistry);

            var previousKeys = previousRegistry.SelectMany(x => x.GeneratedAssetKeys).AssertNotEmpty().ToList().AssertTrue(x => x.Any());
            var currentKeys = currentRegistry.SelectMany(x => x.GeneratedAssetKeys).AssertNotEmpty().ToList().AssertTrue(x => x.Any());

            {
                // None of the Second Registry Generated Keys should exist in the First Registry.
                void KeysDoesNotContain(IEnumerable<Guid> collection, Guid y) => collection.AssertDoesNotContain(x => x == y);

                // The two sets of Generated Code ought not to overlap whatsoever.
                currentKeys.ForEach(x => KeysDoesNotContain(previousKeys, x));
                previousKeys.ForEach(x => KeysDoesNotContain(currentKeys, x));
            }

            {
                // Newly Generated Code will exist, should also be able to verify this.
                void FileExists(string path) => path.AssertFileExists();
                currentKeys.Select(x => Path.Combine(currentRegistry.OutputDirectory, x.RenderGeneratedFileName())).ToList().ForEach(FileExists);
            }

            {
                // Previously Generated Code will have been Purged, should be able to verify this.
                void FileDoesNotExist(string path) => path.AssertFileDoesNotExist();
                previousKeys.Select(x => Path.Combine(previousRegistry.OutputDirectory, x.RenderGeneratedFileName())).ToList().ForEach(FileDoesNotExist);
            }
        }

        [Fact]
        public void Incomplete_Set_Should_Regenerate_Code()
        {
            VerifySingleCodeGenerationPass(out var registry);

            string VerifyFileAndDelete(OrToolsSatGeneratedSyntaxTreeRegistry current)
            {
                string GetGeneratedFilePath(Guid currentKey) => Path.Combine(
                    current.OutputDirectory, currentKey.RenderGeneratedFileName()
                );

                var key = current.AssertNotNull().SelectMany(_ => _.GeneratedAssetKeys).AssertNotEmpty().First();
                var path = GetGeneratedFilePath(key).AssertFileExists();
                File.Delete(path);
                return path;
            }

            VerifyFileAndDelete(registry).AssertFileDoesNotExist();

            VerifySingleCodeGenerationPass(out var secondRegistry);

            VerifyDifferentRegistries(registry, secondRegistry);
        }

        [Fact]
        public void Altered_Or_Corrupted_Set_Should_Regenerate_Code()
        {
            VerifySingleCodeGenerationPass(out var registry);

            string VerifyFileAndRewrite(OrToolsSatGeneratedSyntaxTreeRegistry current)
            {
                string GetGeneratedFilePath(Guid currentKey) => Path.Combine(
                    current.OutputDirectory, currentKey.RenderGeneratedFileName()
                    );

                var key = current.AssertNotNull().SelectMany(_ => _.GeneratedAssetKeys).AssertNotEmpty().First();
                var path = GetGeneratedFilePath(key).AssertFileExists();
                using (var fs = File.Open(path, FileMode.Truncate, FileAccess.Write, FileShare.Read))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write($"{key:D}");
                    }
                }

                return path;
            }

            VerifyFileAndRewrite(registry).AssertFileExists();

            VerifySingleCodeGenerationPass(out var secondRegistry);

            VerifyDifferentRegistries(registry, secondRegistry);
        }

        [Fact]
        public void Generates_When_Google_OrTools_Version_Updates()
        {
            VerifySingleCodeGenerationPass(out var registry);

            // TODO: TBD: could almost inject some code into a generic handler...
            void VerifyRegistryAndUpdate()
            {
                Version Parse(string s) => Version.Parse(s);

                // We need an instance of the Service Manager for what we are about to do here.
                var serviceManager = new CodeGenerationServiceManager(registry.OutputDirectory, RegistryFileName);
                // The Version will surely have updated from `0.0.0´ ... Remember, we also want it in Three Parts.
                serviceManager.Registry.GoogleOrToolsVersion = Parse("0.0.0");
                serviceManager.TrySave();
            }

            VerifyRegistryAndUpdate();

            VerifySingleCodeGenerationPass(out var secondRegistry);

            VerifyDifferentRegistries(registry, secondRegistry);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                void DisposeOfStringWriter(StringWriter writer)
                {
                    OutputHelper.WriteLine($"{writer.AssertNotNull().GetStringBuilder()}");
                    writer.Dispose();
                }

                StringWriters.AssertNotNull().AssertNotEmpty().ToList().ForEach(DisposeOfStringWriter);

                Manager?.Dispose();

                // There is only one, but Dispose of it Conditionally.
                void DisposeOutputDirectory(string path) => Directory.Delete(path, true);
                GetRange(OutputDirectory).Where(Directory.Exists).ToList().ForEach(DisposeOutputDirectory);
            }

            base.Dispose(disposing);
        }
    }
}
