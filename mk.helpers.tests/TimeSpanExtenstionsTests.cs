using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mk.helpers;

namespace mk.helpers.tests
{
    [TestClass]
    public class TimeSpanExtensionsTests
    {
        [TestMethod]
        public void RoundToNearest_RoundsCorrectly()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(5).Add(TimeSpan.FromMicroseconds(500)); // 5.5 seconds
            TimeSpan rounded = timeSpan.RoundToNearest(TimeSpan.FromSeconds(1)); // Round to nearest second

            Assert.AreEqual(TimeSpan.FromSeconds(5), rounded);
        }

        [TestMethod]
        public void ToHumanTimeString_ConvertsCorrectly()
        {
            TimeSpan millisecondsSpan = TimeSpan.FromMilliseconds(500);
            Assert.AreEqual("500 milliseconds", millisecondsSpan.ToHumanTimeString());

            TimeSpan secondsSpan = TimeSpan.FromSeconds(120);
            Assert.AreEqual("2 minutes", secondsSpan.ToHumanTimeString());

            TimeSpan minutesSpan = TimeSpan.FromMinutes(150);
            Assert.AreEqual("2.5 hours", minutesSpan.ToHumanTimeString());

            TimeSpan hoursSpan = TimeSpan.FromHours(48);
            Assert.AreEqual("2 days", hoursSpan.ToHumanTimeString());
        }

        [TestMethod]
        public void Sum_CalculatesSumCorrectly()
        {
            var timeSpans = new List<TimeSpan>
            {
                TimeSpan.FromMinutes(5),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromHours(2)
            };

            TimeSpan sum = timeSpans.Sum();

            Assert.AreEqual("02:05:30", sum.ToString());
        }

        [TestMethod]
        public void Multiply_MultipliesCorrectly()
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(10);
            TimeSpan multiplied = timeSpan.Multiply(3);

            Assert.AreEqual(TimeSpan.FromMinutes(30), multiplied);
        }
    }
}
