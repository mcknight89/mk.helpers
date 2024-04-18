using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers.tests
{

    [TestClass]
    public class PerfTimerTests
    {
        [TestMethod]
        public void PerfTimer_StartStop()
        {
            PerfTimer timer = new PerfTimer();

            timer.Start();
            // Simulate some work
            System.Threading.Thread.Sleep(100);
            timer.Stop();

            Assert.IsTrue(timer.Elapsed.TotalMilliseconds >= 100, "Elapsed time should be at least 100 milliseconds.");
        }

        [TestMethod]
        public void PerfTimer_Reset()
        {
            PerfTimer timer = new PerfTimer();

            timer.Start();
            // Simulate some work
            System.Threading.Thread.Sleep(100);
            timer.Reset();

            Assert.IsTrue(timer.Elapsed.TotalMilliseconds < 100, "Elapsed time should be reset after calling Reset().");
        }

        [TestMethod]
        public void PerfTimer_Interval()
        {
            PerfTimer timer = new PerfTimer();

            timer.Start();
            // Simulate some work
            System.Threading.Thread.Sleep(100);
            timer.Interval();
            // Simulate more work
            System.Threading.Thread.Sleep(200);
            timer.Stop();

            // Elapsed time should be the sum of both intervals
            Assert.IsTrue(timer.AllTimesElapsed.TotalMilliseconds >= 300, "Elapsed time should be at least 300 milliseconds.");
        }

        [TestMethod]
        public void PerfTimer_Execute()
        {
            PerfTimer timer = new PerfTimer();
            var test = false;
            timer.Execute(() =>
            {
                test = true;
            });

            Assert.IsTrue(test, "Execute worked.");
        }

        [TestMethod]
        public void PerfTimer_Disabled()
        {
            PerfTimer timer = new PerfTimer(enabled: false);

            timer.Start();
            // Simulate some work
            System.Threading.Thread.Sleep(100);
            timer.Stop();

            Assert.IsTrue(timer.Elapsed.TotalMilliseconds == 0, "Timer should not record time when disabled.");
        }

        [TestMethod]
        public void PerfTimer_AllTimes()
        {
            PerfTimer timer = new PerfTimer();

            timer.Start();
            // Simulate some work
            System.Threading.Thread.Sleep(100);
            timer.Interval();
            // Simulate more work
            System.Threading.Thread.Sleep(200);
            timer.Stop();

            // The number of recorded times should be 2
            Assert.AreEqual(2, timer.AllTimes.Count, "AllTimes collection should contain 2 recorded times.");

            // Ensure all recorded times are greater than or equal to 100 milliseconds
            foreach (var time in timer.AllTimes)
            {
                Assert.IsTrue(time.TotalMilliseconds >= 100, "Each recorded time should be at least 100 milliseconds.");
            }
        }


        [TestMethod]
        public void PerfTimer_CheckpointTest()
        {
            var timer = PerfTimer.StartNew();

            Thread.Sleep(10);

            timer.AddCheckpoint("Timer 1");

            Thread.Sleep(20);

            timer.AddCheckpoint("Timer 2");

            timer.AddCheckpoint("Timer 3");
            timer.RemoveCheckpoint("Timer 3");

            timer.AddCheckpoint("Timer 4", TimeSpan.FromSeconds(1));

            timer.ExecuteWithCheckpoint("Timer 5", () =>
            {
                Thread.Sleep(30);
            });

            timer.Stop();

            var total = timer.AllTimesElapsed;
            var checkpoints = timer.GetCheckpoints();

            Assert.IsTrue(total.TotalMilliseconds >= 30, "Total time should be at least 30 milliseconds.");
            Assert.IsTrue(checkpoints["Timer 1"].TotalMilliseconds >= 10, "Timer 1 should be at least 10 milliseconds.");
            Assert.IsTrue(checkpoints["Timer 2"].TotalMilliseconds >= 20, "Timer 2 should be at least 20 milliseconds.");
            Assert.IsFalse(checkpoints.ContainsKey("Timer 3"), "Timer 3 should not be present in the checkpoints.");
            Assert.IsTrue(checkpoints["Timer 4"].TotalSeconds == 1, "Timer 4 should be exactly 1 second.");
            Assert.IsTrue(checkpoints["Timer 5"].TotalMilliseconds >= 30, "Timer 5 should be at least 30 milliseconds.");
        }

        [TestMethod]
        public void PerfTimer_ClearCheckpoints()
        {
            var timer = PerfTimer.StartNew();

            Thread.Sleep(10);

            timer.AddCheckpoint("Timer 1");

            Thread.Sleep(20);

            timer.AddCheckpoint("Timer 2");

            timer.ClearCheckpoints();

            timer.AddCheckpoint("Timer 3");

            timer.Stop();

            var total = timer.AllTimesElapsed;
            var checkpoints = timer.GetCheckpoints();

            Assert.IsTrue(total.TotalMilliseconds >= 10, "Total time should be at least 10 milliseconds.");
            Assert.IsFalse(checkpoints.ContainsKey("Timer 1"), "Timer 1 should not be present in the checkpoints.");
            Assert.IsFalse(checkpoints.ContainsKey("Timer 2"), "Timer 2 should not be present in the checkpoints.");
            Assert.IsTrue(checkpoints["Timer 3"].TotalMilliseconds >= 20, "Timer 3 should be at least 20 milliseconds.");
        }

        [TestMethod]
        public void PerfTimer_ClearCheckpointsDisabled()
        {
            var timer = PerfTimer.StartNew().Stop();

            timer.AddCheckpoint("Timer 1");
            timer.AddCheckpoint("Timer 2");
            timer.AddCheckpoint("Timer 3");
            timer.ClearCheckpoints();

            timer.Stop().Clear();

            var total = timer.AllTimesElapsed;
            var checkpoints = timer.GetCheckpoints();

            Assert.IsTrue(total.TotalMilliseconds == 0, "Total time should be 0 milliseconds.");
            Assert.IsFalse(checkpoints.ContainsKey("Timer 1"), "Timer 1 should not be present in the checkpoints.");
            Assert.IsFalse(checkpoints.ContainsKey("Timer 2"), "Timer 2 should not be present in the checkpoints.");
            Assert.IsFalse(checkpoints.ContainsKey("Timer 3"), "Timer 3 should not be present in the checkpoints.");
        }


    }
}
