using System;

namespace Kingdom.OrTools.ConstraintSolver.Samples
{
    using Google.OrTools.ConstraintSolver;
    using NUnit.Framework;
    using OrTools.Samples;

    /// <summary>
    /// Performs some basic unit testing.
    /// </summary>
    public class BasicTests : TestFixtureBase
    {
        /// <summary>
        /// Verifies that the Flatten functionality works as expected.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Test]
        [TestCase(0, 10)]
        [TestCase(10, 0)]
        [TestCase(10, 10)]
        public void VerifyThatFlattenWorksCorrectly(int x, int y)
        {
            // Assumes that at least one dimension actually has dimension.
            Assert.That(x, Is.GreaterThanOrEqualTo(0));
            Assert.That(y, Is.GreaterThanOrEqualTo(0));
            Assert.That(x + y, Is.GreaterThan(0));

            var xmax = Math.Max(x, 1);
            var ymax = Math.Max(y, 1);

            var arr = new IntVar[xmax, ymax];

            Assert.That(arr.GetLength(0), Is.EqualTo(xmax));
            Assert.That(arr.GetLength(1), Is.EqualTo(ymax));

            var flattened = arr.Flatten();

            Assert.That(flattened, Has.Length.EqualTo(xmax*ymax));
        }
    }
}
