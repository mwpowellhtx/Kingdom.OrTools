using NUnit.Framework;

namespace Kingdom.Constraints.Samples
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public abstract class TestFixtureBase
    {
        /// <summary>
        /// Sets up the test fixture prior to running all unit tests.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
        }

        /// <summary>
        /// Tears down the test fixture after running all unit tests.
        /// </summary>
        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
        }

        /// <summary>
        /// Sets up just prior to running each unit test.
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
        }

        /// <summary>
        /// Tears down just after running each unit test.
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
        }
    }
}
