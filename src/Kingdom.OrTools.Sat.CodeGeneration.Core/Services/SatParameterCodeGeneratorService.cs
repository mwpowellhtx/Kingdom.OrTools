using System;
using System.IO;
using System.Reflection;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using static Path;
    using static String;
    using static SatParameterCodeGeneratorService.Constants;

    /// <inheritdoc />
    public class SatParameterCodeGeneratorService : SatParameterCodeGeneratorServiceBase
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Represents some Internal Constants used during the Service resolution.
        /// </summary>
        internal static class Constants
        {
            /// <summary>
            /// &quot;.&quot;
            /// </summary>
            public const string dot = ".";

            /// <summary>
            /// &quot;proto&quot;
            /// </summary>
            public const string proto = nameof(proto);

            /// <summary>
            /// &quot;sat&quot;
            /// </summary>
            public const string sat = nameof(sat);

            /// <summary>
            /// &quot;sat_parameters&quot;
            /// </summary>
            public const string sat_parameters = nameof(sat_parameters);
        }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets the ResourcePath, which, in this case, is expected to be relative to the
        /// requesting context.
        /// </summary>
        /// <inheritdoc />
        internal override string ResourcePath
        {
            get
            {
                var contentDirectoryPath = typeof(Google.OrTools.Sat.CpSolver).Assembly.GetContentDirectoryPath();

                // TODO: TBD: I think this would be accurate, we need to know about the Executing Assembly, not so much `this´ one...
                var execAssemblyDirectoryPath = GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var relativeDirectoryPath = execAssemblyDirectoryPath.GetRelativePath(contentDirectoryPath);

                var satParametersProtoFileName = Join(dot, sat_parameters, proto);

                return Combine(relativeDirectoryPath, sat, satParametersProtoFileName);
            }
        }

        /// <inheritdoc />
        internal override Type ServiceType { get; } = typeof(SatParameterCodeGeneratorService);

        /// <inheritdoc />
        internal override Stream SatParametersProtocolBufferStream => File.Open(
            ResourcePath, FileMode.Open, FileAccess.Read, FileShare.Read
        );
    }
}
