using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
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
    }
}
