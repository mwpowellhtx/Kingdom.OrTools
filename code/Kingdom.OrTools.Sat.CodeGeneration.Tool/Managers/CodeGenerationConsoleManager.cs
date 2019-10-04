using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CompilationUnitSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Code.Generation.Roslyn;
    using Microsoft.CodeAnalysis;
    using NConsole.Options;
    using Validation;
    using static CodeGenerationConsoleManager.Constants;
    using static SatParameterCodeGeneratorService;
    using static String;
    using static SearchOption;
    using static StringComparison;
    using ICompilationUnitSyntaxDictionary = IDictionary<Guid, CompilationUnitSyntax>;
    using IRenderedCompilationUnitDictionary = IDictionary<Guid, string>;
    using CompilationUnitSyntaxPair = KeyValuePair<Guid, string>;

    /// <inheritdoc cref="ICodeGenerationConsoleManager"/>
    internal class CodeGenerationConsoleManager : OptionSetConsoleManager, ICodeGenerationConsoleManager
    {
        // ReSharper disable InconsistentNaming
        internal static class Constants
        {
            private const string Dot = ".";

            /// <summary>
            /// &quot;.g&quot;
            /// </summary>
            internal static string g = $"{Dot}{nameof(g)}";

            /// <summary>
            /// &quot;.cs&quot;
            /// </summary>
            internal static string cs = $"{Dot}{nameof(cs)}";
        }
        // ReSharper restore InconsistentNaming

        /// <inheritdoc />
        public Switch DebugMessagesSwitch { get; }

        /// <inheritdoc />
        public Switch VersionSwitch { get; }

        /// <inheritdoc />
        public Switch GoogleOrToolsVersionSwitch { get; }

        /// <inheritdoc />
        public Variable<string> OutputDirectoryVar { get; }

        /// <inheritdoc />
        public Variable<string> GeneratedCodeRegistryFileVar { get; }

        internal static ICodeGenerationConsoleManager Instance => new CodeGenerationConsoleManager(Console.Out);

        /// <summary>
        /// &quot;Kingdom OrTools Constraint Programming Satisfaction Code Generation&quot;
        /// </summary>
        private const string ConsoleManagerName = "Kingdom OrTools Constraint Programming Satisfaction Code Generation";

        /// <summary>
        /// 1
        /// </summary>
        internal const int ErrorGeneratingCode = 1;

        /// <summary>
        /// 2
        /// </summary>
        internal const int MustSpecifyOutputDirectory = 2;

        /// <summary>
        /// 3
        /// </summary>
        internal const int MustSpecifyRegistryFileName = 3;

        /// <inheritdoc />
        internal CodeGenerationConsoleManager(TextWriter writer, TextWriter errorWriter = null)
            : base(ConsoleManagerName, writer, DefaultHelpPrototype, DefaultHelpDescription, errorWriter)
        {
            DebugMessagesSwitch = Options.AddSwitch("d|debug", "Debug messages should be written.");

            // We do not need any Clean Switches after all. Instead, we factor Cleaning in terms of an MSBuild Target.
            VersionSwitch = Options.AddSwitch("v|version", "Shows the application version.");

            GoogleOrToolsVersionSwitch = Options.AddSwitch("or-tools-version", "Shows the Google.OrTools version.");

            OutputDirectoryVar = Options.AddVariable<string>("output-directory|out-dir|od"
                , "The Output Directory where generate code will land.");

            GeneratedCodeRegistryFileVar = Options.AddVariable<string>("generated-code-registry-file|registry-file|rf"
                , "The Generated Code Registry File.");

            // TODO: TBD: may fill in some blanks with expected and required switches, responses, etc.
            Levels = new ErrorLevelCollection
            {
                // TODO: TBD: somehow isolate how much more of an error there might have been, i.e. internally reported exception is sufficient?
                {
                    MustSpecifyOutputDirectory,
                    () => !VersionSwitch
                          && IsNullOrEmpty(OutputDirectoryVar),
                    () => "Must specify an Output Directory."
                },
                {
                    MustSpecifyRegistryFileName,
                    () => !VersionSwitch
                          && !IsNullOrEmpty(OutputDirectoryVar)
                          && IsNullOrEmpty(GeneratedCodeRegistryFileVar),
                    () => "Must specify a Registry File Name."
                },
                {
                    ErrorGeneratingCode,
                    () => !VersionSwitch
                          && !(IsNullOrEmpty(OutputDirectoryVar)
                               || IsNullOrEmpty(GeneratedCodeRegistryFileVar))
                          && GenerateCode() == ErrorGeneratingCode,
                    () => "There was an error Generating the Code."
                },
                DefaultErrorLevel
            };
        }

        private void ReportException(Exception ex)
        {
            var exceptionLevel = 1;
            while (ex != null)
            {
                ErrorWriter.WriteLine($"{exceptionLevel}: {ex.Message}");
                ErrorWriter.WriteLine($"{exceptionLevel}: {ex.StackTrace}");

                ex = ex.InnerException;
                exceptionLevel++;
            }
        }

        // TODO: TBD: might look at doing this in a base class...
        private int? ShowVersions()
        {
            string RenderVersionString()
            {
                Assembly GetAssembly() => Assembly.GetExecutingAssembly();

                Version GetAssemblyVersion() => GetAssembly().GetName().Version;

                string GetAssemblyInformationalVersion() => GetAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

                bool StringEquals(string a, string b, StringComparison comparison = InvariantCultureIgnoreCase)
                    => string.Equals(a, b, comparison);

                var version = $"{GetAssemblyVersion()}";
                var informational = GetAssemblyInformationalVersion();

                return IsNullOrEmpty(informational) || StringEquals(informational, version)
                    ? version
                    : $"{version} ({informational})";
            }

            /* There is `a lot going on´ behind this properly. Do not take this one lightly. Per
             * the Google version strategy, only using three-parts. The critical bit is that our
             * Code Generation Targets are also leveraging these bits in order to inform where we
             * find the Protocol Buffer `.proto´ specification. */

            string RenderGoogleOrToolsVersion() => $"{GoogleOrToolsVersion.ToString(3)}";

            int? result = null;

            // ReSharper disable once InvertIf
            if (VersionSwitch || GoogleOrToolsVersionSwitch)
            {
                if (VersionSwitch)
                {
                    Writer.WriteLine(RenderVersionString());
                }

                if (GoogleOrToolsVersionSwitch)
                {
                    Writer.WriteLine(RenderGoogleOrToolsVersion());
                }

                result = DefaultErrorLevel;
            }

            return result;
        }

        /// <summary>
        /// Loads the <paramref name="registry"/> and Purges Generated Code given
        /// <paramref name="serviceManager"/>.
        /// </summary>
        /// <param name="serviceManager"></param>
        /// <param name="registry"></param>
        private void LoadAndPurgeGeneratedCode(CodeGenerationServiceManager serviceManager
            , out OrToolsSatGeneratedSyntaxTreeRegistry registry)
        {
            if (DebugMessagesSwitch)
            {
                Writer.WriteLine("Loading and purging registry.");
            }

            /* Should not need to re-load any ServiceManager Registries,
             * since this is done inherently by the SM itself. */
            registry = serviceManager.Registry;

            if (!registry.Any())
            {
                if (DebugMessagesSwitch)
                {
                    Writer.WriteLine("There are no Registry entries.");
                }

                return;
            }

            // Work around output or reference parameters.
            var local = registry;
            var outputDirectory = local.OutputDirectory;
            var anUpdateOccurred = local.GoogleOrToolsVersion < GoogleOrToolsVersion;

            // Purge All if any of them are missing.
            bool ShouldPurgeRegistry(GeneratedSyntaxTreeDescriptor descriptor)
            {
                /* Easy Purge to make by Version first, followed closely by whether
                 * any previously registered files have themselves disappeared. */

                bool WhetherSomeFilesAreMissing()
                {
                    var someFilesAreMissing = descriptor.GeneratedAssetKeys
                        .Select(y => Path.Combine(outputDirectory, y.RenderGeneratedFileName()))
                        .Any(filePath => !File.Exists(filePath));
                    return someFilesAreMissing;
                }

                var should = anUpdateOccurred || WhetherSomeFilesAreMissing();

                if (DebugMessagesSwitch)
                {
                    Writer.WriteLine($"Should{(should ? " " : " not ")}purge generated code.");
                }

                return should;
            }

            registry.PurgeWhere(ShouldPurgeRegistry);
        }

        /// <summary>
        /// We need to Evaluate a further and much more in depth Purge opportunity given the
        /// <paramref name="registry"/> and set of <paramref name="renderedCompilationUnits"/>.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="renderedCompilationUnits"></param>
        /// <returns></returns>
        private bool TryEvaluateCompilationUnits(OrToolsSatGeneratedSyntaxTreeRegistry registry
            , IRenderedCompilationUnitDictionary renderedCompilationUnits)
        {
            // Pretty sure we want to do an Ordinal comparison here. But this is a function of the Invocation.
            bool StringEquals(string a, string b, StringComparison comparisonType)
                => !(a == null || b == null)
                   && string.Equals(a.Trim(), b.Trim(), comparisonType);

            bool TryDecomposeCompilationUnitSyntaxPair(CompilationUnitSyntaxPair pair
                , out string generatedCode) => !IsNullOrEmpty(generatedCode = pair.Value);

            // This is a bit involved, but this gains us sufficient accounting of Generated Code.
            bool ThereAreCodeGenerationInconsistencies(GeneratedSyntaxTreeDescriptor _)
            {
                var outputDirectory = registry.OutputDirectory;

                var filePaths = Directory.EnumerateFiles(outputDirectory, $"*{g}{cs}", TopDirectoryOnly);

                // Obtain any Previously Existing Baseline Generated Code in the form of a Stream Factory.
                var streamFactories = filePaths.Select(filePath => (Func<Stream>)(
                    () => File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                ));

                // Which has the added benefit of deferring Stream operations until we absolutely need them.
                foreach (var streamFactory in streamFactories)
                {
                    string baselineGeneratedCode;

                    /* And which also defers the actual delegate formation given the nature of the LINQ
                     * provider. Performance may vary from OS to OS, from Run-time to Run-time. */
                    using (var fs = streamFactory.Invoke())
                    {
                        // Do this operation Once and Only Once for the breadth of the candidate set of Generated Code.
                        using (var sr = new StreamReader(fs))
                        {
                            baselineGeneratedCode = sr.ReadToEndAsync().Result;
                        }
                    }

                    // Continue when Code Generation Consistency checks can be verified.
                    if (renderedCompilationUnits.Any(
                        pair => TryDecomposeCompilationUnitSyntaxPair(pair, out var x)
                                && StringEquals(x, baselineGeneratedCode, Ordinal)))
                    {
                        continue;
                    }

                    if (DebugMessagesSwitch)
                    {
                        Writer.WriteLine("Generated code inconsistencies discovered, purge required.");
                    }

                    // Otherwise, break, we know the set to be Inconsistent, i.e. re-Generation required.
                    return true;
                }

                if (DebugMessagesSwitch)
                {
                    Writer.WriteLine("Generated code found to be consistent, purge not required.");
                }

                // If everything checked out, All Consistent, then we are Clear to Bypass Code Generation.
                return false;
            }

            var areNonePreviouslyGenerated = !registry.SelectMany(x => x.GeneratedAssetKeys).Any();
            var areObviouslyDifferent = registry.SelectMany(x => x.GeneratedAssetKeys).Count() != renderedCompilationUnits.Count;
            var thereAreCodeGenerationInconsistencies = registry.Any(ThereAreCodeGenerationInconsistencies);

            var shouldPurge = areNonePreviouslyGenerated || areObviouslyDifferent || thereAreCodeGenerationInconsistencies;

            registry.PurgeWhere(_ => shouldPurge);

            return shouldPurge;
        }

        /// <summary>
        /// Gets the Registry.
        /// </summary>
        public OrToolsSatGeneratedSyntaxTreeRegistry Registry { get; private set; }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private int? GenerateCode()
        {
            void EnsureDirectoryExists(string path)
            {
                if (Directory.Exists(path))
                {
                    return;
                }

                Directory.CreateDirectory(path);
            }

            try
            {
                string outputDirectory = OutputDirectoryVar;
                string generatedCodeRegistryFile = GeneratedCodeRegistryFileVar;

                EnsureDirectoryExists(outputDirectory);

                if (DebugMessagesSwitch)
                {
                    Writer.WriteLine($"Output Directory is `{outputDirectory}´, registry file name is `{generatedCodeRegistryFile}´.");
                }

                var serviceManager = new CodeGenerationServiceManager(outputDirectory, generatedCodeRegistryFile);

                LoadAndPurgeGeneratedCode(serviceManager, out var registry);

                Registry = registry;

                registry.AssumesTrue(x => ReferenceEquals(x, serviceManager.Registry));

                // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                var service = new SatParameterCodeGeneratorService {};

                var descriptor = service.Descriptor;

                service.CodeGenerationVisitor.Visit(descriptor);

                var compilationUnits = service.CodeGenerationVisitor.CompilationUnits;

                if (DebugMessagesSwitch)
                {
                    Writer.WriteLine(
                        $"There are {compilationUnits.Count} potential new compilation units as compared"
                        + $" to {registry.SelectMany(x => x.GeneratedAssetKeys).Count()} old ones."
                    );
                }

                // TODO: TBD: this is what we are really talking about...
                // TODO: TBD: we need to find an exact match, otherwise, we reject the current set and regenerate...
                // TODO: TBD: no need to go ad-hoc compiling any previously generated code... that is entirely unnecessary.
                var renderedCompilationUnits = compilationUnits.ToDictionary(
                    x => x.Key
                    , x => x.Value.NormalizeWhitespace().ToFullString()
                );

                if (TryEvaluateCompilationUnits(registry, renderedCompilationUnits))
                {
                    if (DebugMessagesSwitch)
                    {
                        Writer.WriteLine(
                            "Evaluation complete, there are"
                            + $" {renderedCompilationUnits.Count} rendered compilation units."
                        );
                    }

                    var generatedDescriptor = GeneratedSyntaxTreeDescriptor.Create();

                    foreach (var (key, renderedCompilationUnit) in renderedCompilationUnits)
                    {
                        var filePath = Path.Combine(outputDirectory, key.RenderGeneratedFileName());

                        using (var fs = File.Open(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                        {
                            using (var sw = new StreamWriter(fs))
                            {
                                sw.WriteLine(renderedCompilationUnit);
                            }
                        }

                        generatedDescriptor.GeneratedAssetKeys.Add(key);
                    }

                    registry.Add(generatedDescriptor);

                    if (DebugMessagesSwitch)
                    {
                        Writer.WriteLine(
                            "Service Manager Registry instance"
                            + $" is{(ReferenceEquals(serviceManager.Registry, registry) ? " " : " not ")}the same."
                        );
                    }

                    if (DebugMessagesSwitch)
                    {
                        // TODO: TBD: do we need exposure of the SM Registry?
                        Writer.WriteLine(
                            $"There are {generatedDescriptor.GeneratedAssetKeys.Count} generated items and"
                            + $" {serviceManager.Registry.SelectMany(x => x.GeneratedAssetKeys).Count()} total entries to save."
                        );
                    }

                    serviceManager.TrySave();
                }
            }
            catch (Exception ex)
            {
                ReportException(ex);
                return ErrorGeneratingCode;
            }

            return null;
        }

        public override void Run(out int errorLevel)
        {
            // TODO: TBD: again, some of which could potentially be presented from the base class, i.e. bits concerning `Show Version´ ...
            int? shownVersion;

            switch (shownVersion = ShowVersions())
            {
                default:
                    errorLevel = shownVersion.Value;
                    return;

                case null:
                    base.Run(out errorLevel);
                    break;
            }
        }
    }
}
