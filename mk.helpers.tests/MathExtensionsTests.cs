using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mk.helpers.tests
{
    [TestClass]
    public class MathExtensionsTests
    {
        [TestMethod]
        public void Approach_StopsAtTarget()
        {
            Assert.AreEqual(3f, 1f.Approach(10, 2));
            Assert.AreEqual(10f, 9f.Approach(10, 2));
            Assert.AreEqual(8f, 10f.Approach(0, 2));
            Assert.AreEqual(0f, 1f.Approach(0, 2));
        }

        [TestMethod]
        public void Wrap_HandlesNegatives()
        {
            Assert.AreEqual(4, (-1).Wrap(5));
            Assert.AreEqual(0, 5.Wrap(5));
            Assert.AreEqual(2, 12.Wrap(5));
        }
    }
}
