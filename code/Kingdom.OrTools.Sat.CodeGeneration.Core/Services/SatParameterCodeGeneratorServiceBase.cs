using System;
using System.IO;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Antlr4.Runtime;
    using Protobuf;
    using ProtoDeclContext = Protobuf.ProtoParser.ProtoDeclContext;

    // TODO: TBD: bridge from the git gotten or-tools
    public abstract class SatParameterCodeGeneratorServiceBase
    {
        /// <summary>
        /// Gets the ResourcePath. This may be a path to an Embedded Resource, or an actual
        /// Relative or Absolute File Path to the proto delivered as part of the Package
        /// References.
        /// </summary>
        internal abstract string ResourcePath { get; }

        private CodeGenerationProtoDescriptorVisitor _codeGenerationVisitor;

        internal CodeGenerationProtoDescriptorVisitor CodeGenerationVisitor
            => _codeGenerationVisitor ?? (_codeGenerationVisitor
                   // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
                   = new CodeGenerationProtoDescriptorVisitor { });

        /// <summary>
        /// Gets the ServiceType.
        /// </summary>
        internal abstract Type ServiceType { get; }

        // TODO: TBD: where we actually stream the resource from is another question entirely...
        // TODO: TBD: I think it makes sense to peer into the Google.OrTools assembly itself for its version, at minimum, if not also its path, ideally...
        // TODO: TBD: in order to identify the proto from its package distribution source a it was delivered, instead of baking it into this assembly as an embedded resource...
        /// <summary>
        /// Gets the SatParametersProtocolBufferStream.
        /// </summary>
        internal abstract Stream SatParametersProtocolBufferStream { get; }

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

                // TODO: TBD: the right kind of parsing is going on, we think...
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
    }
}
