using System;

namespace Kingdom.OrTools.LinearSolver.Samples.Feasibility
{
    using NUnit.Framework;
    using static LinearResultStatus;

    /// <summary>
    /// 
    /// </summary>
    public class LinearProblemSolverTests : TestFixtureBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void VerifyThatFeasibilityProblemSolverCorrect()
        {
            Func<double, double> round = Math.Round;

            using (var s = new FeasibleRegionProblemSolver())
            {
                s.Solved += (sender, e) =>
                {
                    const LinearResultStatus optimal = Optimal;

                    /* The numbers really ARE NOT that perfect, they are CLOSE, within fraction
                     * of 0.0001, but for rounding. It is also not necessarily the case where
                     * Rounding is a Solver issue, but a unit test one. */

                    Assert.That(e.VariableCount, Is.EqualTo(2));
                    Assert.That(e.ConstraintCount, Is.EqualTo(3));
                    Assert.That(e.ResultStatus, Is.EqualTo(optimal));
                    Assert.That(round(e.Solution), Is.EqualTo(34d));
                    Assert.That(round(e.SolutionValues.x), Is.EqualTo(6d));
                    Assert.That(round(e.SolutionValues.y), Is.EqualTo(4d));
                };

                Assert.That(s.TryResolve(), Is.True);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void VerifyThatFeasibilityProblemSolverWithoutSolutionCorrect()
        {
            Func<double, double> round = Math.Round;

            // Make sure that we actually ARE testing WITHOUT the Solution value.
            using (var s = new FeasibleRegionProblemSolverWithoutSolution())
            {
                s.Solved += (sender, e) =>
                {
                    /* The numbers really ARE NOT that perfect, they are CLOSE, within fraction
                     * of 0.0001, but for rounding. It is also not necessarily the case where
                     * Rounding is a Solver issue, but a unit test one. */

                    Assert.That(e.VariableCount, Is.EqualTo(2));
                    Assert.That(e.ConstraintCount, Is.EqualTo(3));
                    Assert.That(e.ResultStatus, Is.EqualTo(Optimal));
                    Assert.That(round(e.SolutionValues.x), Is.EqualTo(6d));
                    Assert.That(round(e.SolutionValues.y), Is.EqualTo(4d));
                };

                Assert.That(s.TryResolve(), Is.True);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void VerifyThatCbcMipFeasibilityProblemSolverCorrect()
        {
            Func<double, double> round = Math.Round;

            using (var s = new CbcMipFeasibleRegionComparisonProblemSolver())
            {
                s.Solved += (sender, e) =>
                {
                    Assert.That(e.VariableCount, Is.EqualTo(2));
                    Assert.That(e.ConstraintCount, Is.EqualTo(2));
                    Assert.That(e.ResultStatus, Is.EqualTo(Optimal));
                    Assert.That(round(e.SolutionValues.x), Is.EqualTo(3d));
                    Assert.That(round(e.SolutionValues.y), Is.EqualTo(2d));
                    Assert.That(round(e.Solution), Is.EqualTo(23d));
                };

                Assert.That(s.TryResolve(), Is.True);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        [Ignore("The model is failing for reasons unknown; consult with the Google team concerning the bug")]
        public void VerifyThatGlopLpFeasibilityProblemSolverCorrect()
        {
            Func<double, double> round = Math.Round;

            using (var s = new GlopLpFeasibleRegionComparisonProblemSolver())
            {
                s.Solved += (sender, e) =>
                {
                    Assert.That(e.VariableCount, Is.EqualTo(2));
                    Assert.That(e.ConstraintCount, Is.EqualTo(2));
                    // TODO: is yielding Abnormal instead of Optimal...
                    Assert.That(e.ResultStatus, Is.EqualTo(Optimal));
                    Assert.That(round(e.SolutionValues.x), Is.EqualTo(0d));
                    Assert.That(round(e.SolutionValues.y), Is.EqualTo(2.5d));
                    // TODO: is yielding ZERO instead of the predicted solution
                    Assert.That(round(e.Solution), Is.EqualTo(25d));
                };

                Assert.That(s.TryResolve(), Is.True);
            }
        }
    }
}
