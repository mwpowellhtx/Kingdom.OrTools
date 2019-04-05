using System;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using OrTools.Samples;
    using Sudoku;
    using Xunit;
    using Xunit.Abstractions;
    using static Sudoku.Domain;

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:http://cpg.stparchive.com/Year2015/"/>
    public class SudokuSolverTests : TestFixtureBase
    {
        // ReSharper disable once IdentifierTypo
        public SudokuSolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies the problem represented by <paramref name="theValuesText"/>.
        /// </summary>
        /// <param name="theValuesText"></param>
        private void VerifyProblem(string theValuesText)
        {
            Assert.NotNull(theValuesText);

            Assert.Equal(MaximumValue * MaximumValue, theValuesText.Length);

            void ReportTheValues()
            {
                OutputHelper.WriteLine($"The String: \"{theValuesText}\"");
            }

            ReportTheValues();

            var theValues = theValuesText.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray();

            VerifyProblem(theValues);
        }

        /// <summary>
        /// Verifies the problem represented by <paramref name="theValues"/>.
        /// </summary>
        /// <param name="theValues"></param>
        private void VerifyProblem(int[] theValues)
        {
            ISudokuPuzzle theSolution = null;

            var theProblem = theValues.ToSudokuPuzzle();

            void ReportTheValues()
            {
                string RenderTheValues()
                {
                    return string.Join(@", ", theValues.Select(x => $"{x}"));
                }

                OutputHelper.WriteLine($"The Values: {RenderTheValues()}");
            }

            ReportTheValues();

            theProblem.PrettyPrint(Console.Out);

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
        /// <param name="index"></param>
        /// <see cref="!:http://cpg.stparchive.com/Archive/CPG/CPG02132015p101.php" />
        /// <see cref="!:http://cpg.stparchive.com/Archive/CPG/CPG02132015p128.php" />
        /// <see cref="!:http://www.websudoku.com/"/>
        [Theory
         , InlineData(0)
         , InlineData(1)
         , InlineData(2)
         , InlineData(3)]
        public void Verify_Integer_Array_Problem(int index) => VerifyProblem(
            OrTools.Sudoku.Domain.IntegerArrayProblems.ElementAt(index)
        );

        [Theory
         , InlineData(0)]
        public void Verify_String_Problem(int index) => VerifyProblem(
            OrTools.Sudoku.Domain.StringProblems.ElementAt(index)
        );

        /// <summary>
        /// Verifies that the Embedded Resource may be Loaded and is Not Empty.
        /// </summary>
        [Fact]
        public void Verify_the_embedded_resource()
        {
            Assert.NotEmpty(new EmbeddedXmlTestCases());
        }

        /// <summary>
        /// Verifies that an embedded resource problem solves correctly.
        /// </summary>
        /// <param name="theValuesText"></param>
        /// <param name="theDescription"></param>
        [Theory, ClassData(typeof(EmbeddedXmlTestCases))]
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
