using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using static GoogleOrToolsConstants;
    using static SatParameterCodeGeneratorService.Constants;
    using static String;

    /// <inheritdoc />
    public class SatParameterCodeGeneratorService : SatParameterCodeGeneratorServiceBase
    {
        //// ReSharper disable InconsistentNaming
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
            /// &quot;Resources&quot;
            /// </summary>
            public const string Resources = nameof(Resources);

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

        /// <summary>
        /// Returns the Available Versions from the <see cref="PackagePath"/>. We make several
        /// assumptions concerning this strategy. First, we assume that the Package Directories
        /// are enumerated by Version Numbers. Second, we assume that the Version Numbers are
        /// three part Dot delimited Integers. Third, we are also depending upon a couple of
        /// clutch Build Targets in order to generate the Package Path code for internal use.
        /// </summary>
        /// <returns></returns>
        /// <see cref="GoogleOrToolsConstants"/>
        /// <see cref="PackagePath"/>
        private static IEnumerable<Version> GetAvailableVersions()
            => Directory.EnumerateDirectories(PackagePath)
                .Select(Path.GetFileName)
                .Select(x => x.Split(dot[0]).Select(int.Parse).ToArray())
                .Select(y => new Version(y[0], y[1], y[2]));

        /// <summary>
        /// Gets the Most Recent Google OrTools Version possible.
        /// </summary>
        /// <see cref="GetAvailableVersions"/>
        /// <see cref="Version.Major"/>
        /// <see cref="Version.Minor"/>
        /// <see cref="Version.Build"/>
        internal static Version GoogleOrToolsVersion => GetAvailableVersions()
            .OrderByDescending(x => x.Major).ThenByDescending(x => x.Minor)
            .ThenByDescending(x => x.Build).First();

        /// <inheritdoc />
        internal override string ResourcePath => Join(dot, Resources, Join(dot, sat_parameters, proto));

        /// <inheritdoc />
        internal override Type ServiceType { get; } = typeof(SatParameterCodeGeneratorService);

        /// <summary>
        /// Returns the Manifest Resource Stream given <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <see cref="ResourcePath"/>
        private Stream GetProtocolBufferStream(Type type) => type.Assembly.GetManifestResourceStream(type, ResourcePath);

        /// <inheritdoc />
        /// <see cref="object.GetType"/>
        /// <see cref="GetProtocolBufferStream"/>
        internal override Stream SatParametersProtocolBufferStream => GetProtocolBufferStream(GetType());
    }
}
