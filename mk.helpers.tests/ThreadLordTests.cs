using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mk.helpers.tests
{
    [TestClass]
    public class ThreadLordTests
    {


        [TestMethod]
        public void Enqueue_And_Process_Items()
        {
            int processedCount = 0;
            Action<int> workCallback = item =>
            {
                Interlocked.Increment(ref processedCount);
            };

            using (var threadLord = new ThreadLord<int>(workCallback))
            {
                threadLord.Start();

                for (int i = 0; i < 100; i++)
                {
                    threadLord.Enqueue(i);
                }

                threadLord.WaitAllAndStop();
            }

            Assert.AreEqual(100, processedCount);
        }

        [TestMethod]
        public void Process_With_Limit_Queue()
        {
            int processedCount = 0;
            Action<int> workCallback = item =>
            {
                Interlocked.Increment(ref processedCount);
            };

            using (var threadLord = new ThreadLord<int>(workCallback).LimitQueue(10))
            {
                threadLord.Start();

                for (int i = 0; i < 100; i++)
                {
                    threadLord.Enqueue(i);
                }

                threadLord.WaitAllAndStop();
            }

            Assert.AreEqual(100, processedCount);
        }

        [TestMethod]
        public void Enqueued_Items_Are_Cleared_After_Clear()
        {
            using (var threadLord = new ThreadLord<int>(item => { }))
            {
                threadLord.Start();

                for (int i = 0; i < 100; i++)
                {
                    threadLord.Enqueue(i);
                }

                threadLord.Clear();

                Assert.AreEqual(0, threadLord.Enqueued);
            }
        }

        [TestMethod]
        public void Processed_Items_Are_Counted_Correctly()
        {
            int processedCount = 0;
            Action<int> workCallback = item =>
            {
                Interlocked.Increment(ref processedCount);
            };

            using (var threadLord = new ThreadLord<int>(workCallback))
            {
                threadLord.Start();

                for (int i = 0; i < 100; i++)
                {
                    threadLord.Enqueue(i);
                }

                threadLord.WaitAllAndStop();

                Assert.AreEqual(100, threadLord.Processed);
            }
        }
    }
}
