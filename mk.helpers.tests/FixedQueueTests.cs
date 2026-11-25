using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace mk.helpers.tests
{
    [TestClass]
    public class FixedQueueTests
    {
        [TestMethod]
        public void Enqueue_DropsOldestWhenFull()
        {
            var queue = new FixedQueue<int>(3);
            for (var i = 1; i <= 5; i++)
                queue.Enqueue(i);

            Assert.AreEqual(3, queue.Count);
            CollectionAssert.AreEqual(new[] { 3, 4, 5 }, queue.ToArray());

            queue.Clear();
            Assert.AreEqual(0, queue.Count);
        }

        [TestMethod]
        public void Constructor_RejectsZeroCapacity()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new FixedQueue<int>(0));
        }
    }
}
