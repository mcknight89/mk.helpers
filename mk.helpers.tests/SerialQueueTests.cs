using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace mk.helpers.tests
{
    [TestClass]
    public class SerialQueueTests
    {
        [TestMethod]
        public void Post_RunsInOrder_InvokeWaits()
        {
            using var queue = new SerialQueue("test");
            var seen = new List<int>();

            for (var i = 0; i < 100; i++)
            {
                var n = i;
                queue.Post(() => seen.Add(n));
            }

            var count = queue.Invoke(() => seen.Count);

            Assert.AreEqual(100, count);
            for (var i = 0; i < 100; i++)
                Assert.AreEqual(i, seen[i]);
        }

        [TestMethod]
        public void Invoke_ThrowsToCaller_PostRaisesError()
        {
            using var queue = new SerialQueue("test");
            Exception? raised = null;
            queue.Error += x => raised = x;

            Assert.ThrowsException<InvalidOperationException>(() => queue.Invoke(() => throw new InvalidOperationException("boom")));

            queue.Post(() => throw new ArgumentException("post"));
            queue.Invoke(() => { });
            Assert.IsInstanceOfType(raised, typeof(ArgumentException));
        }

        [TestMethod]
        public void Invoke_FromQueueThread_DoesNotDeadlock()
        {
            using var queue = new SerialQueue("test");

            var result = queue.Invoke(() => queue.Invoke(() => 5));

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void After_RunsLater()
        {
            using var queue = new SerialQueue("test");
            using var ran = new ManualResetEventSlim();

            using var timer = queue.After(TimeSpan.FromMilliseconds(50), () => ran.Set());

            Assert.IsTrue(ran.Wait(TimeSpan.FromSeconds(5)));
        }

        [TestMethod]
        public void After_RunsWhenResultNotKept()
        {
            using var queue = new SerialQueue("test");
            using var ran = new ManualResetEventSlim();

            queue.After(TimeSpan.FromMilliseconds(100), () => ran.Set());
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsTrue(ran.Wait(TimeSpan.FromSeconds(5)));
        }

        [TestMethod]
        public void After_DisposeCancels()
        {
            using var queue = new SerialQueue("test");
            var ran = false;

            var pending = queue.After(TimeSpan.FromMilliseconds(100), () => ran = true);
            pending.Dispose();
            Thread.Sleep(300);
            queue.Invoke(() => { });

            Assert.IsFalse(ran);
        }
    }
}
