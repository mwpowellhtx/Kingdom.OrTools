using System;
using System.IO;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    public class InternalSatParameterCodeGeneratorService : SatParameterCodeGeneratorServiceBase
    {
        /// <inheritdoc />
        internal override string ResourcePath { get; } = @"Resources.sat_parameters.proto";

        /// <summary>
        /// Gets or sets whether to Generate Project Bits. Default is False.
        /// </summary>
        internal bool GenerateProjectBits { get; set; }

        /// <inheritdoc />
        internal override Type ServiceType { get; } = typeof(InternalSatParameterCodeGeneratorService);

        private Stream _satParametersProtocolBufferStream;

        /// <inheritdoc />
        internal override Stream SatParametersProtocolBufferStream
            => _satParametersProtocolBufferStream ?? (_satParametersProtocolBufferStream
                   = ServiceType.Assembly.GetManifestResourceStream(ServiceType, ResourcePath));
    }
}
