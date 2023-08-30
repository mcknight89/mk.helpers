using System;
using System.Collections.Generic;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for working with <see cref="TimeSpan"/> objects.
    /// </summary>
    public static class TimeSpanExtenstions
    {
        /// <summary>
        /// Rounds the given <see cref="TimeSpan"/> to the nearest multiple of the specified <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="a">The <see cref="TimeSpan"/> to be rounded.</param>
        /// <param name="roundTo">The <see cref="TimeSpan"/> multiple to round to.</param>
        /// <returns>The rounded <see cref="TimeSpan"/>.</returns>
        public static TimeSpan RoundToNearest(this TimeSpan a, TimeSpan roundTo)
        {
            long ticks = (long)(Math.Round(a.Ticks / (double)roundTo.Ticks) * roundTo.Ticks);
            return new TimeSpan(ticks);
        }

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> object to a simple human-readable string representation.
        /// </summary>
        /// <param name="span">The <see cref="TimeSpan"/> to be converted.</param>
        /// <param name="significantDigits">The number of significant digits to use for output. Default is 3.</param>
        /// <returns>The human-readable string representation of the <see cref="TimeSpan"/>.</returns>
        public static string ToHumanTimeString(this TimeSpan span, int significantDigits = 3)
        {
            var format = "G" + significantDigits;
            return span.TotalMilliseconds < 1000 ? span.TotalMilliseconds.ToString(format) + " milliseconds"
                : (span.TotalSeconds < 60 ? span.TotalSeconds.ToString(format) + " seconds"
                    : (span.TotalMinutes < 60 ? span.TotalMinutes.ToString(format) + " minutes"
                        : (span.TotalHours < 24 ? span.TotalHours.ToString(format) + " hours"
                                                : span.TotalDays.ToString(format) + " days")));
        }

        /// <summary>
        /// Calculates the sum of a collection of <see cref="TimeSpan"/> objects.
        /// </summary>
        /// <param name="timeSpans">The collection of <see cref="TimeSpan"/> objects.</param>
        /// <returns>The sum of the <see cref="TimeSpan"/> objects.</returns>
        public static TimeSpan Sum(this IEnumerable<TimeSpan> timeSpans)
        {
            TimeSpan sumTillNowTimeSpan = TimeSpan.Zero;

            foreach (TimeSpan timeSpan in timeSpans)
            {
                sumTillNowTimeSpan += timeSpan;
            }

            return sumTillNowTimeSpan;
        }

        /// <summary>
        /// Multiplies a <see cref="TimeSpan"/> by a specified multiplier.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to be multiplied.</param>
        /// <param name="multiplier">The multiplier value.</param>
        /// <returns>The result of the multiplication as a new <see cref="TimeSpan"/>.</returns>
        public static TimeSpan Multiply(this TimeSpan timeSpan, int multiplier)
        {
            return TimeSpan.FromTicks(timeSpan.Ticks * multiplier);
        }
    }
}
