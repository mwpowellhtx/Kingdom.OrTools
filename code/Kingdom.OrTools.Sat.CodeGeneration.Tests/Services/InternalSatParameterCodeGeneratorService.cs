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
        /// <see cref="InternalSatParameterCodeGeneratorService"/>
        internal override Type ServiceType { get; } = typeof(InternalSatParameterCodeGeneratorService);

        /// <inheritdoc />
        internal override Stream SatParametersProtocolBufferStream => ServiceType.Assembly.GetManifestResourceStream(ServiceType, ResourcePath);

    }
}
