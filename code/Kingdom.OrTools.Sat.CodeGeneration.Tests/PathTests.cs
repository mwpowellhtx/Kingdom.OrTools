using System.IO;
using System.Linq;
using System.Reflection;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Xunit;
    using Xunit.Abstractions;
    using static Path;

    public class PathTests : TestFixtureBase
    {
        public PathTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies a couple of academic and even interesting test cases.
        /// Then we verify the truly interesting test case for what we aim to accomplish.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="otherPath"></param>
        /// <param name="expectedRelativePath"></param>
        [Theory
         , InlineData(@"D:\This\Is\A\Test", @"D:\This\Is", @"..\..\..\Is")
         , InlineData(@"D:\This\Is", @"D:\This\Is\A\Test", @"A\Test")
         // ReSharper disable once StringLiteralTypo
         , InlineData(@"D:\Source\My.Project\bin\Debug\netstandard2.0"
             , @"D:\Source\packages\Google.OrTools\1.2.3456\content\sat\sat_parameters.proto"
             , @"..\..\..\..\packages\Google.OrTools\1.2.3456\content\sat\sat_parameters.proto")
        ]
        public void Verify_Relative_Path(string basePath, string otherPath, string expectedRelativePath)
        {
            OutputHelper.WriteLine($"`{nameof(basePath)}´ = `{basePath}´");
            OutputHelper.WriteLine($"`{nameof(otherPath)}´ = `{otherPath}´");
            OutputHelper.WriteLine($"`{nameof(expectedRelativePath)}´ = `{expectedRelativePath}´");

            var actualRelativePath = basePath.GetRelativePath(otherPath);
            OutputHelper.WriteLine($"`{nameof(actualRelativePath)}´ = `{actualRelativePath}´");

            actualRelativePath.AssertEqual(expectedRelativePath);
        }

        [Fact]
        public void Verify_Package_Reference_Paths()
        {
            var actualDirectoryFullName = typeof(Google.OrTools.Sat.CpSolver).Assembly.GetContentDirectoryPath().AssertNotNull();

            var actualProtoPath = Combine(actualDirectoryFullName, "sat", "sat_parameters.proto").AssertFileExists();

            OutputHelper.WriteLine($"`{nameof(actualDirectoryFullName)}´ = `{actualDirectoryFullName}´");
            OutputHelper.WriteLine($"`{nameof(actualProtoPath)}´ = `{actualProtoPath}´");

            var referenced = GetType().Assembly.GetReferencedAssemblies()
                .Where(x => x.FullName.Contains("Google.OrTools")).ToArray();

            var loadedName = referenced.SingleOrDefault().AssertNotNull();

            OutputHelper.WriteLine($"Loaded assembly version: `{loadedName.Version}´");

            Assembly.Load(loadedName).AssertNotNull();
        }
    }
}
