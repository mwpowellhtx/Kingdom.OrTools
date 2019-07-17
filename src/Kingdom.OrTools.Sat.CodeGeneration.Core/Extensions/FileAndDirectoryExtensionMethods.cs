using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using static Path;
    using static String;

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
            var path = assembly.Location;

            // TODO: TBD: with this approach, would likely never see the GetDirectoryInfo() alternative.
            DirectoryInfo GetFileInfoDirectory() => File.Exists(path) ? new FileInfo(path).Directory : null;
            DirectoryInfo GetDirectoryInfo() => Directory.Exists(path) ? new DirectoryInfo(path) : null;

            bool TryGetDirectoryInfo(out DirectoryInfo di) => (di = GetFileInfoDirectory() ?? GetDirectoryInfo()) != null;

            if (!TryGetDirectoryInfo(out var directory))
            {
                throw new ArgumentException($"`{nameof(path)}´ was not a valid file or directory.");
            }

            /* Expecting a NuGet Package Reference in a path something like these:
             *  `packages\google.ortools.runtime.win-x64\7.1.6720\runtimes\win-x64\lib\netstandard2.0\Google.OrTools.dll´
             *  `packages\google.ortools\version\content\sat\sat_parameters.proto´ */

            //  std        lib     rt      rts     ver     pkg     pkgs
            if (directory?.Parent?.Parent?.Parent?.Parent?.Parent?.Parent == null)
            {
                throw new ArgumentException($"Unexpected `{nameof(path)}´ unable to resolve Content directory.");
            }

            const char dot = '.';

            // Which we know should be the first two elements from the package name itself, literally, i.e. `google.ortools.runtime.signature´.
            var greatGrandParentDirectoryName = directory.Parent.Parent.Parent.Parent.Parent.Name;

            var googlePath = Join($"{dot}", greatGrandParentDirectoryName.Split(dot).Take(2).ToArray());

            const string content = nameof(content);

            return Combine(directory.Parent.Parent.Parent.Parent.Parent.Parent.FullName
                , googlePath, assembly.GetName().Version.ToString(3), content);
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
