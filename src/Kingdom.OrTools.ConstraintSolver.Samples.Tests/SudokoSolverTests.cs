using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using OrTools.Samples;
    using Sudoku;
    using Xunit;
    using Xunit.Abstractions;
    using static Sudoku.Domain;

    // ReSharper disable once IdentifierTypo
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:http://cpg.stparchive.com/Year2015/"/>
    public class SudokoSolverTests : TestFixtureBase
    {
        // ReSharper disable once IdentifierTypo
        public SudokoSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies the problem represented by <paramref name="theValuesText"/>.
        /// </summary>
        /// <param name="theValuesText"></param>
        private static void VerifyProblem(string theValuesText)
        {
            Assert.NotNull(theValuesText);

            Assert.Equal(MaximumValue * MaximumValue, theValuesText.Length);

            var theValues = theValuesText.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();

            VerifyProblem(theValues);
        }

        /// <summary>
        /// Verifies the problem represented by <paramref name="theValues"/>.
        /// </summary>
        /// <param name="theValues"></param>
        private static void VerifyProblem(int[] theValues)
        {
            ISudokuPuzzle theSolution = null;

            var theProblem = theValues.ToSudokuPuzzle();

            ((ISudokuPuzzle) theProblem).PrettyPrint(Console.Out);

            //TODO: introduce time outs to the solver ...
            // Should solve and not time out...
            using (var s = new SudokuProblemSolver(theProblem))
            {
                s.Solved += (sender, e) =>
                {
                    var problemSolver = Assert.IsAssignableFrom<SudokuProblemSolver>(sender);
                    Assert.NotNull(problemSolver.Solution);
                    theSolution = problemSolver.Solution;
                };

                Assert.True(s.TryResolve());
            }

            Assert.NotNull(theSolution);
            Assert.NotSame(theProblem, theSolution);
            Assert.True(theSolution.IsSolved);

            theSolution.PrettyPrint(Console.Out);
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Verify_first_problem()
        {
            // Published: Cape Gazette, Tue, Jun 23 - Thu, Jun 25, 2015, p. 21
            VerifyProblem(new[]
            {
                //====================================
                0, 4, 0, /*|*/ 0, 7, 9, /*|*/ 0, 6, 0,
                5, 0, 0, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
                0, 9, 8, /*|*/ 5, 0, 0, /*|*/ 2, 0, 0,
                //====================================
                0, 0, 0, /*|*/ 1, 0, 0, /*|*/ 9, 3, 0,
                1, 0, 0, /*|*/ 4, 0, 0, /*|*/ 6, 0, 8,
                0, 0, 9, /*|*/ 0, 0, 0, /*|*/ 0, 0, 1,
                //====================================
                0, 0, 0, /*|*/ 0, 2, 8, /*|*/ 0, 0, 3,
                8, 0, 0, /*|*/ 0, 0, 0, /*|*/ 0, 7, 0,
                4, 6, 0, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="!:http://cpg.stparchive.com/Archive/CPG/CPG02132015p101.php" />
        /// <see cref="!:http://cpg.stparchive.com/Archive/CPG/CPG02132015p128.php" />
        [Fact]
        public void Verify_second_problem()
        {
            // Published: Cape Gazette, Fri, Feb 13 - Mon, Feb 16, 2015, p. 101
            VerifyProblem(new[]
            {
                //====================================
                0, 3, 0, /*|*/ 0, 0, 0, /*|*/ 5, 4, 0,
                0, 0, 0, /*|*/ 0, 5, 0, /*|*/ 8, 0, 0,
                7, 0, 0, /*|*/ 0, 3, 0, /*|*/ 1, 0, 0,
                //====================================
                0, 5, 9, /*|*/ 0, 0, 2, /*|*/ 4, 0, 0,
                8, 0, 0, /*|*/ 0, 0, 0, /*|*/ 0, 9, 7,
                0, 0, 0, /*|*/ 0, 9, 0, /*|*/ 0, 0, 0,
                //====================================
                0, 0, 4, /*|*/ 6, 0, 0, /*|*/ 2, 0, 0,
                0, 8, 7, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
                5, 0, 6, /*|*/ 1, 0, 0, /*|*/ 0, 0, 0,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="!:http://www.sudoku.com/"/>
        [Fact]
        public void Verify_third_problem()
        {
            // Little bit different, more concise representation of the problem.
            var lines = new[]
            {
                "905060340",
                "028000591",
                "000009600",
                "097630104",
                "000974000",
                "406028950",
                "009500000",
                "562000480",
                "083090205",
            };

            VerifyProblem(string.Join(@"", lines));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="!:http://www.websudoku.com/"/>
        [Fact]
        public void Verify_fourth_problem()
        {
            // Published: Cape Gazette, Fri, Feb 13 - Mon, Feb 16, 2015, p. 101
            var theValues = new[]
            {
                0, 0, 0, 0, 0, 5, 1, 9, 0,
                0, 0, 9, 2, 6, 0, 0, 0, 7,
                5, 6, 0, 0, 4, 9, 0, 8, 0,

                6, 1, 0, 0, 0, 8, 0, 0, 4,
                0, 0, 0, 0, 0, 0, 0, 0, 0,
                4, 0, 0, 9, 0, 0, 0, 6, 3,

                0, 2, 0, 8, 7, 0, 0, 4, 9,
                9, 0, 0, 0, 5, 2, 7, 0, 0,
                0, 7, 1, 6, 0, 0, 0, 0, 0,
            };

            VerifyProblem(theValues);
        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Verify_fifth_problem()
        {
            // Published: Cape Gazette, Fri, Jun 26 - Mon, Jun 29, 2015, p. 107
            VerifyProblem(new[]
            {
                //====================================
                0, 2, 1, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
                0, 0, 0, /*|*/ 0, 2, 3, /*|*/ 8, 0, 4,
                0, 0, 0, /*|*/ 6, 0, 0, /*|*/ 0, 0, 0,
                //====================================
                1, 0, 0, /*|*/ 0, 8, 7, /*|*/ 0, 0, 0,
                0, 9, 5, /*|*/ 2, 0, 0, /*|*/ 0, 0, 0,
                8, 0, 0, /*|*/ 0, 0, 0, /*|*/ 9, 6, 0,
                //====================================
                0, 0, 9, /*|*/ 0, 0, 0, /*|*/ 0, 4, 3,
                0, 0, 0, /*|*/ 5, 6, 0, /*|*/ 0, 0, 8,
                6, 0, 4, /*|*/ 0, 7, 0, /*|*/ 2, 0, 0,
            });
        }

        /// <summary>
        /// Supports loading the Sudoku puzzles from embedded resource.
        /// </summary>
        internal class SudokuTestCaseItem
        {
            public string Puzzle { get; set; }
            public string Description { get; set; }
        }

        /// <summary>
        /// Returns the Sudoku puzzles from resource for test purposes.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IEnumerable<SudokuTestCaseItem> GetSudokuTestCaseItems(Type type)
        {
            var assembly = type.Assembly;

            // ReSharper disable once IdentifierTypo, StringLiteralTypo
            const string sudokusXml = @"Sudokus.xml";

            using (var rs = assembly.GetManifestResourceStream(type, sudokusXml))
            {
                Assert.NotNull(rs);

                string GetAttributeValue(XElement element, string attributeName)
                {
                    Assert.NotNull(element);
                    var attribute = element.Attribute(attributeName);
                    Assert.NotNull(attribute);
                    return attribute.Value;
                }

                using (var reader = new StreamReader(rs, Encoding.UTF8))
                {
                    var loaded = XElement.Load(reader);

                    // TODO: TBD: consider phasing out SudokuTestCaseItem for simple Xunit friendly object[] range...

                    var loadedItems = loaded.Descendants(@"Sudoku")
                        .Select(x =>
                        {
                            Assert.NotNull(x);
                            return x;
                        })
                        .Select(x => new SudokuTestCaseItem
                        {
                            Puzzle = GetAttributeValue(x, "Puzzle"),
                            Description = GetAttributeValue(x, "Description")
                        });

                    foreach (var x in loadedItems)
                    {
                        yield return x;
                    }
                }
            }
        }

        // TODO: TBD: should consider refactoring into class data...
        /// <summary>
        /// Gets the EmbeddedResourceTestCases.
        /// </summary>
        public static IEnumerable<object[]> EmbeddedResourceTestCases
            => GetSudokuTestCaseItems(typeof(SudokoSolverTests))
                .Select(y => GetRange<object>(y.Description, y.Puzzle).ToArray());

        /// <summary>
        /// Verifies that the embedded resource may be loaded.
        /// </summary>
        [Fact]
        public void Verify_the_embedded_resource()
        {
            Assert.NotEmpty(GetSudokuTestCaseItems(GetType()));
        }

        /// <summary>
        /// Verifies that an embedded resource problem solves correctly.
        /// </summary>
        /// <param name="theValuesText"></param>
        /// <param name="theDescription"></param>
        [Theory, MemberData(nameof(EmbeddedResourceTestCases))]
        public void Verify_embedded_resource_problem(string theDescription, string theValuesText)
        {
            if (!string.IsNullOrEmpty(theDescription))
            {
                OutputHelper.WriteLine(theDescription);
            }

            VerifyProblem(theValuesText);
        }
    }
}
