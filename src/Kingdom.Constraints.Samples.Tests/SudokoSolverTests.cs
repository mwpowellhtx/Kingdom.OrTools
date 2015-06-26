using System;
using System.Linq;
using Kingdom.Constraints.Samples.Sudoku;
using NUnit.Framework;

namespace Kingdom.Constraints.Samples
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:http://cpg.stparchive.com/Year2015/"/>
    public class SudokoSolverTests : TestFixtureBase
    {
        /// <summary>
        /// Verifies the problem represented by <paramref name="theValuesText"/>.
        /// </summary>
        /// <param name="theValuesText"></param>
        private static void VerifyProblem(string theValuesText)
        {
            Assert.That(theValuesText, Is.Not.Null);

            Assert.That(theValuesText, Has.Length.EqualTo(9*9));

            var theValues = theValuesText.ToCharArray()
                .Select(x => int.Parse(x.ToString())).ToArray();

            VerifyProblem(theValues);
        }

        /// <summary>
        /// Verifies the problem repesented by <paramref name="theValues"/>.
        /// </summary>
        /// <param name="theValues"></param>
        private static void VerifyProblem(int[] theValues)
        {
            ISudokuPuzzle theSolution = null;

            var theProblem = theValues.ToSudokuPuzzle();

            //TODO: introduce time outs to the solver ...
            // Should solve and not time out...
            using (var sps = new SudokuProblemSolver(theProblem))
            {
                sps.Solved += (sender, e) =>
                {
                    var problemSolver = ((SudokuProblemSolver)sender);
                    Assert.That(problemSolver.Solution, Is.Not.Null);
                    theSolution = problemSolver.Solution;
                };

                Assert.That(sps.TryResolve());
            }

            Assert.That(theSolution, Is.Not.Null);
            Assert.That(theSolution, Is.Not.SameAs(theProblem));
            Assert.That(theSolution.IsSolved);

            theSolution.PrettyPrint(Console.Out);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
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
        [Test]
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
        [Test]
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
        [Test]
        public void Verify_fourth_problem()
        {
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
    }
}
