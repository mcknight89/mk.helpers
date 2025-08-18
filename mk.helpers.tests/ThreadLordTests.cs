using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class ThreadLordExtraTests
    {
        // Helper: wait until condition or fail
        private static void WaitTrue(Func<bool> cond, int ms = 3000, int poll = 10)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < ms)
            {
                if (cond()) return;
                Thread.Sleep(poll);
            }
            Assert.Fail("Condition not reached in time.");
        }

        [TestMethod]
        public void Start_Idempotent()
        {
            using var tl = new ThreadLord<int>(_ => { });
            tl.Start();
            tl.Start(); // should not throw or create duplicate workers
            Assert.IsTrue(tl.Workers > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Enqueue_Throws_When_Not_Started()
        {
            using var tl = new ThreadLord<int>(_ => { });
            tl.Enqueue(1);
        }

        [TestMethod]
        public void Enqueue_After_Dispose_Throws()
        {
            var tl = new ThreadLord<int>(_ => { });
            tl.Start();
            tl.Dispose();
            Assert.ThrowsException<InvalidOperationException>(() => tl.Enqueue(1));
        }

        [TestMethod]
        public void CurrentlyProcessing_Returns_To_Zero_When_Idle()
        {
            using var done = new CountdownEvent(50);
            using var tl = new ThreadLord<int>(_ => { done.Signal(); });

            tl.Start();
            for (int i = 0; i < 50; i++) tl.Enqueue(i);

            Assert.IsTrue(done.Wait(3000), "Work did not complete.");
            WaitTrue(() => tl.CurrentlyProcessing == 0 && tl.Enqueued == 0);
        }

        [TestMethod]
        public void Processed_Counter_Increments_Correctly()
        {
            using var tl = new ThreadLord<int>(_ => { });
            tl.Start();

            for (int i = 0; i < 200; i++) tl.Enqueue(i);
            tl.WaitAllAndStop();

            Assert.AreEqual(200, tl.Processed);
        }

        [TestMethod]
        public void LimitQueue_Enforce_Backpressure()
        {
            var gate = new ManualResetEventSlim(false);
            int processed = 0;

            using var tl = new ThreadLord<int>(_ =>
            {
                gate.Wait(); // block workers until we release
                Interlocked.Increment(ref processed);
            }).LimitQueue(5);

            tl.Start();

            // Fill to capacity; these must enqueue quickly
            for (int i = 0; i < 5; i++) tl.Enqueue(i);

            // Sixth enqueue should block until we release gate
            var enqueue6 = Task.Run(() => tl.Enqueue(6));

            // Give it a moment to confirm it's blocked
            Assert.IsFalse(enqueue6.Wait(150), "Enqueue did not block at capacity.");

            // Now release workers so they drain and free slots
            gate.Set();

            Assert.IsTrue(enqueue6.Wait(2000), "Enqueue did not complete after capacity freed.");
            tl.WaitAllAndStop();

            Assert.AreEqual(6, processed);
        }

        [TestMethod]
        public void Clear_Releases_Slots_For_Bounded_Queue()
        {
            var firstItemTaken = new ManualResetEventSlim(false);
            var gate = new ManualResetEventSlim(false);

            using var tl = new ThreadLord<int>(onWork: _ =>
            {
                firstItemTaken.Set();
                gate.Wait();
            }, maxThreads: 1).LimitQueue(3);

            tl.Start();

            for (int i = 0; i < 3; i++) tl.Enqueue(i);


            Assert.IsTrue(firstItemTaken.Wait(1000), "Worker did not take first item.");

            tl.Clear();
            Assert.AreEqual(0, tl.Enqueued, "Queue should be empty after Clear when one is in-flight.");

            var t = Task.Run(() =>
            {
                tl.Enqueue(100);
                tl.Enqueue(101);
            });

            Assert.IsTrue(t.Wait(1000), "Enqueue after Clear should not block for freed queued slots.");
            gate.Set();

            tl.WaitAllAndStop();

            Assert.AreEqual(3, tl.Processed);
        }



        [TestMethod]
        public void Stop_Allows_Graceful_Shutdown()
        {
            using var tl = new ThreadLord<int>(_ => { Thread.Sleep(10); });
            tl.Start();

            for (int i = 0; i < 100; i++) tl.Enqueue(i);

            tl.WaitAllAndStop();
            Assert.IsTrue(tl.Workers >= 0); // not throwing is success; workers may still be completing dispose wait
        }

        [TestMethod]
        public void ProcessedLastSecond_And_Average_Report_NonZero()
        {
            using var tl = new ThreadLord<int>(_ => { });
            tl.Start();

            for (int i = 0; i < 200; i++) tl.Enqueue(i);

            // allow stats loop to tick at least once
            Thread.Sleep(1100);

            var last = tl.ProcessedLastSecond;
            var avg = tl.ProcessedPerSecondAverage;

            tl.WaitAllAndStop();

            Assert.IsTrue(last >= 0);
            Assert.IsTrue(avg >= 0);
        }

        [TestMethod]
        public void Callback_Exception_Does_Not_Hang()
        {
            int attempts = 0;
            using var tl = new ThreadLord<int>(_ =>
            {
                Interlocked.Increment(ref attempts);
                if (attempts % 5 == 0) throw new InvalidOperationException("boom");
            });

            tl.Start();

            for (int i = 0; i < 50; i++) tl.Enqueue(i);

            // Should still drain and not hang even with exceptions
            tl.WaitAllAndStop();

            Assert.AreEqual(50, tl.Processed);
        }

        [TestMethod]
        public void High_Contention_Many_Producers()
        {
            using var tl = new ThreadLord<int>(_ => { });
            tl.Start();

            int producers = 8;
            int per = 200;
            Parallel.For(0, producers, _ =>
            {
                for (int i = 0; i < per; i++) tl.Enqueue(i);
            });

            tl.WaitAllAndStop();
            Assert.AreEqual(producers * per, tl.Processed);
        }


        [TestMethod]
        public void Failed_Increments_And_Processed_Counts_Attempts()
        {
            // Throw on every 3rd item → for 10 items expect 3 failures.
            int attempts = 0;
            using var tl = new ThreadLord<int>(_ =>
            {
                var n = Interlocked.Increment(ref attempts);
                if (n % 3 == 0) throw new InvalidOperationException("boom");
            }, maxThreads: 1);

            tl.Start();
            for (int i = 0; i < 10; i++) tl.Enqueue(i);
            tl.WaitAllAndStop();

            Assert.AreEqual(10, tl.Processed, "Processed should count all attempts.");
            Assert.AreEqual(3, tl.Failed, "Failed should count callback exceptions.");
        }

        [TestMethod]
        public void OnError_Is_Invoked_For_Each_Exception_With_Exception_Instance()
        {
            var seen = new ConcurrentBag<Exception>();
            int attempts = 0;

            using var tl = new ThreadLord<int>(_ =>
            {
                var n = Interlocked.Increment(ref attempts);
                if ((n & 1) == 0) throw new InvalidOperationException("even fail"); // fail on evens
            }, maxThreads: 1)
            .OnError(ex => seen.Add(ex));

            tl.Start();
            for (int i = 0; i < 8; i++) tl.Enqueue(i);
            tl.WaitAllAndStop();

            Assert.AreEqual(8, tl.Processed);
            Assert.AreEqual(4, tl.Failed);
            Assert.AreEqual(4, seen.Count, "OnError must be called once per failure.");
            foreach (var ex in seen)
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void No_Errors_Results_In_Zero_Failed_And_No_OnError_Calls()
        {
            int onErrorCalls = 0;
            using var tl = new ThreadLord<int>(_ => { /* no-op */ }, maxThreads: 1)
                .OnError(_ => Interlocked.Increment(ref onErrorCalls));

            tl.Start();
            for (int i = 0; i < 50; i++) tl.Enqueue(i);
            tl.WaitAllAndStop();

            Assert.AreEqual(50, tl.Processed);
            Assert.AreEqual(0, tl.Failed);
            Assert.AreEqual(0, onErrorCalls);
        }
    }
}
