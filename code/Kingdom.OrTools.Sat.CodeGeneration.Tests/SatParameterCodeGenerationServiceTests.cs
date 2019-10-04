using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Protobuf;
    using Xunit;
    using Xunit.Abstractions;
    using static String;
    using IKeyedCompilationUnitEnumerable = IEnumerable<KeyValuePair<Guid, Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax>>;

    public class SatParameterCodeGenerationServiceTests : SatParameterCodeGeneratorServiceTestFixtureBase<SatParameterCodeGeneratorService>
    {
        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        protected override SatParameterCodeGeneratorService Service => new SatParameterCodeGeneratorService { };

        public SatParameterCodeGenerationServiceTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <inheritdoc />
        protected override string VerifyResourceExists(string resourcePath)
        {
            var serviceType = Service.AssertNotNull().GetType();

            var assembly = serviceType.Assembly;

            using (var stream = assembly.GetManifestResourceStream(serviceType, resourcePath))
            {
                stream.AssertNotNull();
            }

            return resourcePath;
        }

        /// <summary>
        /// Reports the <paramref name="pair"/> for logging purposes.
        /// </summary>
        /// <param name="pair"></param>
        /// <remarks>The results look pretty good thus far. Rounding out the aspects we will need
        /// in order to inform any sort of pattern for rolling up a parameters string.</remarks>
        private void Report(KeyValuePair<Guid, CompilationUnitSyntax> pair)
        {
            const string crLf = "\r\n";

            var key = pair.Key;
            var source = $"{pair.Value.GetText()}";

            OutputHelper.WriteLine(Join(crLf
                , $"<{nameof(pair)}>"
                , $"    <{nameof(key)}>{key:D}</{nameof(key)}>"
                , $"    <{nameof(source)}>"
                , source
                , $"    </{nameof(source)}>"
                , $"</{nameof(pair)}>"
            ));
        }

        /// <summary>
        /// Verifies that the Parsed Descriptor is correct.
        /// </summary>
        /// <remarks>This is fast approaching the pinnacle moment where we can best support
        /// Sat Solver Parameters at a language level in as generic a manner as possible.
        /// The rub will be how we need to organize and deliver the code generation during
        /// the build process. Worst case, we will need to consider a secondary internal
        /// solution that builds the generators themselves for internal publication. Then,
        /// we subscribe to those in order to actually do the code generation delivering
        /// into the Sat Parameters project itself. Best possible case is that we can
        /// simply leverage the CG assemblies as `project references´, but I have some
        /// reservations whether that may work, considering CG aspects involving file
        /// locks, but we will see.</remarks>
        [Fact]
        public void Verify_Parsed_Descriptor()
        {
            var service = Service.AssertNotNull();

            /* Some of which is internal syntactic sugar, of course, but the essentials are there,
             * considering how this might apply to a Code.Generation.Roslyn Assembly Generator. */

            CodeGenerationProtoDescriptorVisitor VisitProtoDescriptor(ProtoDescriptor descriptor)
                => service.CodeGenerationVisitor.AssertNotNull().Initialize(x => x.Visit(descriptor));

            /* We think this is pretty much it. Rolling up a Descriptor should roll up the source, which
             * rolls up the stream, which should roll up the Resource Path, which we have mapped to the
             * appropriate location. */

            var visitor = VisitProtoDescriptor(service.Descriptor.AssertNotNull());

            // We should be able to verify some internal consistency features, distinct numbers, counts, etc.
			visitor.AssertTrue(x => x.EnumCount > 0);
			visitor.AssertTrue(x => x.ClassCount > 0);
			visitor.TotalCount.AssertEqual(visitor.EnumCount + visitor.ClassCount);
            visitor.ParameterNumbers.Distinct().ToArray().AssertTrue(x => x.Length == visitor.ClassCount);

            // While we can do this in a single fluent statement, it is worthwhile separating these.
            var dictionary = visitor.CompilationUnits.AssertNotNull();
            dictionary.AssertTrue(x => x.Count == visitor.TotalCount);
            dictionary.AssertTrue(x => x.Keys.Distinct().Count() == visitor.TotalCount);
            dictionary.AssertTrue(x => x.Values.All(y => !(y is null)));

            // This is a bit more robust of an inquiry, but it leaves the pairs intact.
            IKeyedCompilationUnitEnumerable GetCompilationUnits<TNode>(IKeyedCompilationUnitEnumerable compilationUnits)
                where TNode : SyntaxNode
                => compilationUnits.Where(x => x.Value.DescendantNodes().OfType<TNode>().Any());

            /* So, we are not really `Initializing´ with the ForEach, so much as we do want
             * a Report prior to vetting the Count. We want to vet the generated units. */

            GetCompilationUnits<EnumDeclarationSyntax>(dictionary)
                .ToList().Initialize(x => x.ForEach(Report))
                .AssertTrue(x => x.Count == visitor.EnumCount)
                ;

            GetCompilationUnits<ClassDeclarationSyntax>(dictionary)
                .ToList().Initialize(x => x.ForEach(Report))
                .AssertTrue(x => x.Count == visitor.ClassCount)
                ;
        }
    }
}
