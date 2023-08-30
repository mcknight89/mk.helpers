using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class QueueExtensionsTests
    {
        [TestMethod]
        public void DequeueChunk_ConcurrentQueue_Success()
        {
            var queue = new ConcurrentQueue<int>();
            for (int i = 1; i <= 10; i++)
            {
                queue.Enqueue(i);
            }
            var result = queue.DequeueChunk(5);
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public void DequeueChunk_Queue_Success()
        {
            var queue = new Queue<string>();
            for (int i = 1; i <= 10; i++)
            {
                queue.Enqueue($"Item {i}");
            }
            var result = queue.DequeueChunk(3);
            Assert.AreEqual(3, result.Count());
        }
    }
}
