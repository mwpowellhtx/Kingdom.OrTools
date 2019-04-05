using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using OrTools.Samples;
    using Sudoku;
    using Xunit;
    using Xunit.Abstractions;
    using static Sudoku.Domain;

    //// TODO: TBD: may want to consolidate a couple of these Sudoku assets since it seems I have a bit of duplication going on...
    // ReSharper disable once CommentTypo, IdentifierTypo
    /// <summary>
    /// Basically many examples drawn from Cape Gazette newspapers.
    /// </summary>
    /// <see cref="!:http://cpg.stparchive.com/Year2015/">Newspaper Archive of Cape Gazette, Lewes, Delaware</see>
    /// <inheritdoc />
    public class SudokoSolverTests : TestFixtureBase
    {
        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <inheritdoc />
        public SudokoSolverTests(ITestOutputHelper outputHelper)
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

            var theValues = theValuesText.ToCharArray()
                .Select(x => int.Parse(x.ToString())).ToArray();

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

            void Show(string s) => ShowMessage(s);

            ((ISudokuPuzzle) theProblem).PrettyPrint(Show);

            //TODO: introduce time outs to the solver ...
            // Should solve and not time out...
            using (var s = new SudokuProblemSolver(theProblem))
            {
                s.Solved += (sender, e) =>
                {
                    Assert.True(sender is SudokuProblemSolver);
                    var problemSolver = (SudokuProblemSolver) sender;
                    Assert.NotNull(problemSolver.Solution);
                    theSolution = problemSolver.Solution;
                };

                Assert.True(s.TryResolve());
            }

            Assert.NotNull(theSolution);
            Assert.NotSame(theProblem, theSolution);
            Assert.True(theSolution.IsSolved);

            theSolution.PrettyPrint(Show);
        }

        /// <summary>
        /// Verifies the First Problem.
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
        /// Verifies the Second Problem.
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
        /// Verifies the Third Problem.
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
        /// Verifies the Fourth Problem.
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
        /// Verifies the Fifth Problem.
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
        /// Verifies that the embedded resource may be loaded.
        /// </summary>
        [Fact]
        public void Verify_the_embedded_resource()
        {
            // We could even verify the precise count, but this will do for now.
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
                ShowMessage(theDescription);
            }

            VerifyProblem(theValuesText);
        }
    }
}
