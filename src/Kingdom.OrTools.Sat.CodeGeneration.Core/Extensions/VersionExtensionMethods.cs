using System;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    /// <summary>
    /// Version extension methods.
    /// </summary>
    internal static class VersionExtensionMethods
    {
        /// <summary>
        /// &quot;0.0&quot;
        /// </summary>
        private const string DefaultVersionString = "0.0";

        /// <summary>
        /// Parses the Version given <paramref name="s"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static Version ParseVersion(this string s) => new Version(s ?? DefaultVersionString);
    }
}
