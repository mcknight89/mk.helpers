using System;
using System.Collections.Generic;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for working with <see cref="DateTime"/> values.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the suffix for the day in a <see cref="DateTime"/> (e.g., "st", "nd", "rd", "th").
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        /// <returns>The day suffix.</returns>
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

        /// <summary>
        /// Rounds a <see cref="DateTime"/> to the nearest multiple of a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        /// <param name="span">The <see cref="TimeSpan"/> to round to.</param>
        /// <returns>The rounded <see cref="DateTime"/> value.</returns>
        public static DateTime Round(this DateTime date, TimeSpan span)
        {
            var ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        /// <summary>
        /// Floors a <see cref="DateTime"/> to the nearest multiple of a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        /// <param name="span">The <see cref="TimeSpan"/> to floor to.</param>
        /// <returns>The floored <see cref="DateTime"/> value.</returns>
        public static DateTime Floor(this DateTime date, TimeSpan span)
        {
            var ticks = (date.Ticks / span.Ticks);
            return new DateTime(ticks * span.Ticks);
        }

        /// <summary>
        /// Ceils a <see cref="DateTime"/> to the nearest multiple of a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        /// <param name="span">The <see cref="TimeSpan"/> to ceil to.</param>
        /// <returns>The ceiled <see cref="DateTime"/> value.</returns>
        public static DateTime Ceil(this DateTime date, TimeSpan span)
        {
            var ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }


        /// <summary>
        /// Gets the start of the week for a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> value.</param>
        /// <param name="startOfWeek">The starting day of the week.</param>
        /// <returns>The start of the week <see cref="DateTime"/> value.</returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }


        /// <summary>
        /// Gets the last day of the week for a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> value.</param>
        /// <returns>The last day of the week.</returns>
        public static DateTime EndOfWeek(this DateTime dt)
        {
            return dt.StartOfWeek(DayOfWeek.Monday).AddDays(6);
        }

        /// <summary>
        /// Gets the first day of the month for a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> value.</param>
        /// <returns>The first day of the month.</returns>
        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
        }

        /// <summary>
        /// Gets the last day and time of the month for a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> value.</param>
        /// <returns>The last day of the month.</returns>
        public static DateTime EndOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 23, 59, 59, 999).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Gets the last day of the month for a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dt">The <see cref="DateTime"/> value.</param>
        /// <returns>The last day of the month.</returns>
        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0).AddMonths(1).AddDays(-1);
        }


        /// <summary>
        /// Gets the end of the day for a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        /// <returns>The end of the day.</returns>
        public static DateTime EndOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        /// <summary>
        /// Gets the beginning of the day for a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        /// <returns>The beginning of the day.</returns>
        public static DateTime BeginningOfTheDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        /// <summary>
        /// Gets the age based on the difference between a birthdate and the current date.
        /// </summary>
        /// <param name="birthDate">The birthdate.</param>
        /// <returns>The calculated age.</returns>
        public static int GetAge(this DateTime birthDate)
        {
            return GetAge(birthDate, DateTime.Now);
        }

        /// <summary>
        /// Gets the age of a person based on their birthdate and a given reference date.
        /// </summary>
        /// <param name="birthDate">The birthdate of the person.</param>
        /// <returns>The age of the person.</returns>
        public static int GetAge(this DateTime birthDate, DateTime at)
        {
            if (at < birthDate)
                throw new ArgumentOutOfRangeException(nameof(at), "At date can not be before birthDate");

            var hadBirthday = birthDate.Month < at.Month
                || (birthDate.Month == at.Month && birthDate.Day <= at.Day);

            return at.Year - birthDate.Year - (hadBirthday ? 0 : 1);
        }


        /// <summary>
        /// Truncates a <see cref="DateTime"/> to a specified resolution.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        /// <param name="resolution">The resolution to truncate to.</param>
        /// <returns>The truncated <see cref="DateTime"/> value.</returns>
        public static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        /// <summary>
        /// Generates a range of dates between two <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DateTime"/> values within the range.</returns>
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


        /// <summary>
        /// Fills in missing dates in a sequence of data using a default item factory.
        /// </summary>
        /// <typeparam name="T">The type of the data items.</typeparam>
        /// <param name="allDates">The list of all dates.</param>
        /// <param name="sourceData">The source data collection.</param>
        /// <param name="dateSelector">A function to extract the date from a data item.</param>
        /// <param name="defaultItemFactory">A function to create a default item based on a date.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with missing dates filled in.</returns>
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
        /// <summary>
        /// Converts a <see cref="DateTime"/> to a human-readable time ago format.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/> value.</param>
        /// <returns>The time ago representation.</returns>
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


        // <summary>
        // Converts a <see cref="TimeSpan"/> to a negative value if it is not already negative.
        /// <summary>
        /// <returns>An negative <see cref="TimeSpan"/></returns>
        public static TimeSpan ToNegative(this TimeSpan span)
        {
            if (span.Ticks < 0)
            {
                return span;
            }
            return new TimeSpan(-span.Ticks);
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a positive value if it is not already positive.
        /// </summary>
        /// <returns>an positive <see cref="TimeSpan"/></returns>
        public static TimeSpan ToPositive(this TimeSpan span)
        {
            if (span.Ticks > 0)
            {
                return span;
            }
            return new TimeSpan(-span.Ticks);
        }



    }
}
