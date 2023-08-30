using System;
using System.Collections.Generic;

namespace mk.helpers
{
    public static class DateTimeExtensions
    {
        public static string GetDaySuffix(this DateTime date)
        {
            switch (date.Day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
        public static DateTime Round(this DateTime date, TimeSpan span)
        {
            var ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }
        public static DateTime Floor(this DateTime date, TimeSpan span)
        {
            var ticks = (date.Ticks / span.Ticks);
            return new DateTime(ticks * span.Ticks);
        }
        public static DateTime Ceil(this DateTime date, TimeSpan span)
        {
            var ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            return dt.StartOfWeek(DayOfWeek.Monday).AddDays(6);
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static DateTime EndOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static DateTime BeginningOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
        public static int GetAge(this DateTime birthDate)
        {
            return GetAge(birthDate, DateTime.Now);
        }

        public static int GetAge(this DateTime birthDate, DateTime at)
        {
            if (at < birthDate)
                throw new ArgumentOutOfRangeException(nameof(at), "At date can not be before birthDate");

            var hadBirthday = birthDate.Month < at.Month
                || (birthDate.Month == at.Month && birthDate.Day <= at.Day);

            return at.Year - birthDate.Year - (hadBirthday ? 0 : 1);
        }
        public static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }


        public static IEnumerable<DateTime> GetDateRange(this DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                yield return startDate;

            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }


        //Source: https://stackoverflow.com/questions/1468637/filling-in-missing-dates-using-a-linq-group-by-date-query
        public static IEnumerable<T> FillInEmptyDates<T>(this IEnumerable<DateTime> allDates, IEnumerable<T> sourceData, Func<T, DateTime> dateSelector, Func<DateTime, T> defaultItemFactory)
        {
            // iterate through the source collection
            var iterator = sourceData.GetEnumerator();
            iterator.MoveNext();

            // for each date in the desired list
            foreach (var desiredDate in allDates)
            {
                // check if the current item exists and is the 'desired' date
                if (iterator.Current != null &&
                    dateSelector(iterator.Current) == desiredDate)
                {
                    // if so then return it and move to the next item
                    yield return iterator.Current;
                    iterator.MoveNext();

                    // if source data is now exhausted then continue
                    if (iterator.Current == null)
                    {
                        continue;
                    }

                    // ensure next item is not a duplicate 
                    if (dateSelector(iterator.Current) == desiredDate)
                    {
                        throw new Exception("More than one item found in source collection with date " + desiredDate);
                    }
                }
                else
                {
                    // if the current 'desired' item doesn't exist then
                    // create a dummy item using the provided factory
                    yield return defaultItemFactory(desiredDate);
                }
            }
        }

        public static string TimeAgo(this DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }
    }
}
