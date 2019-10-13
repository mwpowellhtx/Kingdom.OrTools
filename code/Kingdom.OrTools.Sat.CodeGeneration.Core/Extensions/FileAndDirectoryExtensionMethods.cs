using System;
using System.IO;
using System.Reflection;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using static Path;
    using static StringComparison;

    internal static class FileAndDirectoryExtensionMethods
    {
        /// <summary>
        /// This could be any <see cref="Assembly"/>, but we are expecting the Google.OrTools
        /// Assembly, in particular. The tricky part is, we receive the Runtime Assembly, from
        /// which we must infer details concerning the API surface area assembly. We can make
        /// some assumptions about the version based on the <see cref="AssemblyName"/>
        /// we are given, and the directory levels, with predetermined knowledge of the
        /// package directory structure. From there we append the appropriate version and
        /// content bits in order to achieve a reasonable path from which to begin working
        /// with Google Protocol Buffer files.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetContentDirectoryPath(this Assembly assembly)
        {
            /* We had been expecting a NuGet Package Reference in a path something like these
             * since at least version 7.1:
             *  `packages\google.ortools.runtime.win-x64\7.1.6720\runtimes\win-x64\lib\netstandard2.0\Google.OrTools.dll´
             *  `packages\google.ortools\version\content\sat\sat_parameters.proto´ */

            /* We are not sure what changed in recent weeks, but now the path for
             * `typeof(Google.OrTool.Sat.CpModel).Assembly.Location´ resolves to
             * `packages\google.ortools\7.4.7247\lib\netstandard2.0\Google.OrTools.dll´,
             * which is originally what we might have expected. However, why the sudden change?
             * More importantly, should we account for potentially either use case being the case? */

            /* Investigating what might have changed, as necessary, starting in the Google.OrTools forums.
             * https://groups.google.com/forum/#!topic/or-tools-discuss/3O45yhSF4Uk / 7.4 release
             * If that leads me to Microsoft forums, so be it, if necessary. Would like to make a
             * more informed decision how better to handle this issue other than my own empirical
             * observations. However, at the same time, how big a decision does this need to be. */

            /* In prior versions, we predicated our assumptions based on a firm runtime path.
             * However, this assumption appears to have broken in the latest build? So...
             * what assumptions can we depend upon? */

            var path = assembly.Location;

            /* Ferret out an appropriate Parent Directory this way instead of relying
             * on any consistent Directory Parent depths. */

            // ReSharper disable once StringLiteralTypo
            const string googleOrTools = "google.ortools";
            const string content = nameof(content);

            DirectoryInfo GetPackagesRootDirectory(DirectoryInfo given)
            {
                var found = given;

                for (;
                    // ReSharper disable once MergeSequentialChecks
                    !(found == null || found.Parent == null)
                    && !found.Name.StartsWith(googleOrTools, InvariantCultureIgnoreCase);
                    found = found.Parent)
                {
                    // Do nothing, let the loop control sort it out for us.
                }

                return found?.Parent;
            }

            var packageDirectory = GetPackagesRootDirectory(new FileInfo(path).Directory);

            var resultPath = Combine(packageDirectory.FullName
                , googleOrTools
                , assembly.GetName().Version.ToString(3)
                , content
            );

            return resultPath.ToLower();
        }

        /// <summary>
        /// Gets the Relative Path from <paramref name="basePath"/> and
        /// <paramref name="otherPath"/>.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="otherPath"></param>
        /// <returns></returns>
        public static string GetRelativePath(this string basePath, string otherPath)
        {
            basePath = GetFullPath(basePath);
            otherPath = GetFullPath(otherPath);

            const string backSlash = "\\";

            // Requires trailing backward slash for path.
            if (!basePath.EndsWith(backSlash))
            {
                basePath += backSlash;
            }

            const string forwardSlash = "/";

            var baseUri = new Uri(basePath);
            var otherUri = new Uri(otherPath);

            var relativeUri = baseUri.MakeRelativeUri(otherUri);

            // Uri use forward slashes so convert back to backward slashes.
            return $"{relativeUri}".Replace(forwardSlash, backSlash);
        }
    }
}
