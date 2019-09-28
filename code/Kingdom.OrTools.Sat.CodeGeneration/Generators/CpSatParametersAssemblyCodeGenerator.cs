using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingdom.OrTools.Sat.CodeGeneration.Generators
{
    using CodeGeneration;
    using Code.Generation.Roslyn;
    using Microsoft.CodeAnalysis;
    using Validation;

    public class CpSatParametersAssemblyCodeGenerator : AssemblyCodeGenerator
    {
        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        /// <summary>
        /// Gets the Service Instance.
        /// </summary>
        private SatParameterCodeGeneratorService Service { get; } = new SatParameterCodeGeneratorService { };

        /// <inheritdoc />
        public CpSatParametersAssemblyCodeGenerator(AttributeData attributeData)
            : base(attributeData)
        {
        }

        /// <inheritdoc />
        public override Task GenerateAsync(AssemblyTransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var s = Service;

            void GenerateCallback()
            {
                // TODO: TBD: sprinkles in a bit of Validation as well...
                // TODO: TBD: would be better if `Validation´ actually supported a more Fluent style...
                Assumes.Present(s);

                // A lot goes in behind the Descriptor parsing the Protocol Buffer file into a usable form.
                var pd = s.Descriptor;
                var visitor = s.CodeGenerationVisitor;

                Assumes.Present(pd);
                Assumes.Present(visitor);

                // Then we want to Visit the Descriptor to glean the critical details for CG.
                visitor.Visit(pd);

                // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                var cgd = new CodeGeneratorDescriptor { };

                Assumes.NotNullOrEmpty(visitor.CompilationUnits);
                Assumes.NotNullOrEmpty(cgd.CompilationUnits);

                foreach (var (k, x) in visitor.CompilationUnits)
                {
                    // TODO: TBD: potential future direction for CGR, we may work by adding a dictionary of CompilationUnitSyntax instances...
                    Verify.Operation(k != Guid.Empty, $"Dictionary key `{nameof(k)}´ should not be Empty.");
                    Assumes.NotNull(x);

                    cgd.CompilationUnits.Add(x);
                }

                Assumes.Present(Descriptors);

                Descriptors.Add(cgd);
            }

            return Task.Run(GenerateCallback, cancellationToken);
        }
    }
}
