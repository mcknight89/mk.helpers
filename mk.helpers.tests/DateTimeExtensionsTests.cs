using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void GetDaySuffix_FirstDay_ReturnsSt()
        {
            var date = new DateTime(2023, 8, 1);
            var suffix = date.GetDaySuffix();

            Assert.AreEqual("st", suffix);
        }

        [TestMethod]
        public void GetDaySuffix_SecondDay_ReturnsNd()
        {
            var date = new DateTime(2023, 8, 2);
            var suffix = date.GetDaySuffix();

            Assert.AreEqual("nd", suffix);
        }

        [TestMethod]
        public void GetDaySuffix_ThirdDay_ReturnsRd()
        {
            var date = new DateTime(2023, 8, 3);
            var suffix = date.GetDaySuffix();

            Assert.AreEqual("rd", suffix);
        }

        [TestMethod]
        public void GetDaySuffix_FourthDay_ReturnsTh()
        {
            var date = new DateTime(2023, 8, 4);
            var suffix = date.GetDaySuffix();

            Assert.AreEqual("th", suffix);
        }

        [TestMethod]
        public void Round_NearestHour_RoundsCorrectly()
        {
            var date = new DateTime(2023, 8, 29, 12, 35, 0);
            var roundedDate = date.Round(TimeSpan.FromHours(1));

            Assert.AreEqual(new DateTime(2023, 8, 29, 13, 0, 0), roundedDate);
        }

        [TestMethod]
        public void Floor_NearestHour_FloorsCorrectly()
        {
            var date = new DateTime(2023, 8, 29, 12, 35, 0);
            var flooredDate = date.Floor(TimeSpan.FromHours(1));

            Assert.AreEqual(new DateTime(2023, 8, 29, 12, 0, 0), flooredDate);
        }

        [TestMethod]
        public void Ceil_NearestHour_CeilsCorrectly()
        {
            var date = new DateTime(2023, 8, 29, 12, 35, 0);
            var ceiledDate = date.Ceil(TimeSpan.FromHours(1));

            Assert.AreEqual(new DateTime(2023, 8, 29, 13, 0, 0), ceiledDate);
        }

        [TestMethod]
        public void StartOfWeek_SundayStart_ReturnsStartOfSameWeek()
        {
            var date = new DateTime(2023, 8, 29); // Monday
            var startOfWeek = date.StartOfWeek(DayOfWeek.Sunday);

            Assert.AreEqual(new DateTime(2023, 8, 27), startOfWeek);
        }

        [TestMethod]
        public void EndOfWeek_Default_ReturnsEndOfSameWeek()
        {
            var date = new DateTime(2023, 8, 29); // Monday
            var endOfWeek = date.EndOfWeek();

            Assert.AreEqual(new DateTime(2023, 9, 3), endOfWeek);
        }

        [TestMethod]
        public void StartOfMonth_Default_ReturnsStartOfSameMonth()
        {
            var date = new DateTime(2023, 8, 29);
            var startOfMonth = date.StartOfMonth();

            Assert.AreEqual(new DateTime(2023, 8, 1, 0, 0, 0), startOfMonth);
        }

        [TestMethod]
        public void EndOfMonth_Default_ReturnsEndOfSameMonth()
        {
            var date = new DateTime(2023, 8, 29);
            var endOfMonth = date.EndOfMonth();

            Assert.AreEqual(new DateTime(2023, 8, 31, 23, 59, 59, 999), endOfMonth);
        }

        [TestMethod]
        public void LastDayOfMonth_Default_ReturnsEndOfSameMonth()
        {
            var date = new DateTime(2023, 8, 29);
            var endOfMonth = date.LastDayOfMonth();

            Assert.AreEqual(new DateTime(2023, 8, 31, 0, 0, 0, 0), endOfMonth);
        }


        [TestMethod]
        public void EndOfTheDay_Default_ReturnsEndOfSameDay()
        {
            var date = new DateTime(2023, 8, 29, 14, 30, 15);
            var endOfTheDay = date.EndOfTheDay();

            Assert.AreEqual(new DateTime(2023, 8, 29, 23, 59, 59, 999), endOfTheDay);
        }

        [TestMethod]
        public void BeginningOfTheDay_Default_ReturnsBeginningOfSameDay()
        {
            var date = new DateTime(2023, 8, 29, 14, 30, 15);
            var beginningOfTheDay = date.BeginningOfTheDay();

            Assert.AreEqual(new DateTime(2023, 8, 29, 0, 0, 0), beginningOfTheDay);
        }


        [TestMethod]
        public void GetAge_Default_ReturnsCorrectAge()
        {
            var birthDate = new DateTime(1990, 5, 15);
            var age = birthDate.GetAge();

            var currentDate = DateTime.Now;
            var expectedAge = currentDate.Year - birthDate.Year - (currentDate.DayOfYear < birthDate.DayOfYear ? 1 : 0);

            Assert.AreEqual(expectedAge, age);
        }

        [TestMethod]
        public void GetAge_CustomReferenceDate_ReturnsCorrectAge()
        {
            var birthDate = new DateTime(1990, 5, 15);
            var referenceDate = new DateTime(2023, 8, 29);
            var age = birthDate.GetAge(referenceDate);

            var expectedAge = referenceDate.Year - birthDate.Year - (referenceDate.DayOfYear < birthDate.DayOfYear ? 1 : 0);

            Assert.AreEqual(expectedAge, age);
        }

        [TestMethod]
        public void Truncate_Default_ReturnsTruncatedDateTime()
        {
            var date = new DateTime(2023, 8, 29, 14, 30, 15);
            var resolution = TimeSpan.FromHours(1).Ticks;

            var truncatedDate = date.Truncate(resolution);

            Assert.AreEqual(new DateTime(2023, 8, 29, 14, 0, 0), truncatedDate);
        }

        [TestMethod]
        public void GetDateRange_Default_ReturnsDateRange()
        {
            var startDate = new DateTime(2023, 8, 29);
            var endDate = new DateTime(2023, 9, 1);
            var dateRange = startDate.GetDateRange(endDate).ToList();

            var expectedDates = new List<DateTime>
            {
                new DateTime(2023, 8, 29),
                new DateTime(2023, 8, 30),
                new DateTime(2023, 8, 31),
                new DateTime(2023, 9, 1)
            };

            CollectionAssert.AreEqual(expectedDates, dateRange);
        }

        [TestMethod]
        public void FillInEmptyDates_Default_ReturnsFilledData()
        {
            var allDates = new List<DateTime>
            {
                new DateTime(2021, 8, 01),
                new DateTime(2021, 8, 02),
                new DateTime(2021, 8, 03),
            };
            var sourceData = new List<string>
            {
                "Data for 01/08/2021",
                "Data for 03/08/2021"
            };
            Func<string, DateTime> dateSelector = item => DateTime.ParseExact(item.Substring(9), "dd/MM/yyyy", null);
            Func<DateTime, string> defaultItemFactory = date => $"Default data for {date:dd/MM/yyyy}";

            var filledData = allDates.FillInEmptyDates(sourceData, dateSelector, defaultItemFactory).ToList();

            var expectedData = new List<string>
            {
                "Data for 01/08/2021",
                "Default data for 02/08/2021",
                "Data for 03/08/2021"
            };

            CollectionAssert.AreEqual(expectedData, filledData);
        }

        [TestMethod]
        public void TimeAgo_Default_ReturnsCorrectTimeAgo()
        {
            var dateTime = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));
            var timeAgo = dateTime.TimeAgo();

            Assert.AreEqual("about 30 minutes ago", timeAgo);
        }

        [TestMethod]
        public void TimeSpan_ToNegative()
        {
            var timeSpan = new TimeSpan(0, 30, 0);
            var negativeTimeSpan = timeSpan.ToNegative();
            Assert.AreEqual(new TimeSpan(0, -30, 0), negativeTimeSpan);


            timeSpan = new TimeSpan(0, -30, 0);
            negativeTimeSpan = timeSpan.ToNegative();
            Assert.AreEqual(new TimeSpan(0, -30, 0), negativeTimeSpan);
        }

        [TestMethod]
        public void TimeSpan_ToPositive()
        {
            var timeSpan = new TimeSpan(0, -30, 0);
            var positiveTimeSpan = timeSpan.ToPositive();
            Assert.AreEqual(new TimeSpan(0, 30, 0), positiveTimeSpan);

            timeSpan = new TimeSpan(0, 30, 0);
            positiveTimeSpan = timeSpan.ToPositive();
            Assert.AreEqual(new TimeSpan(0, 30, 0), positiveTimeSpan);
        }
    }
}
