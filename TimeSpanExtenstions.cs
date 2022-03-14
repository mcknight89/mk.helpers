using System;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    public static  class TimeSpanExtenstions
    {
        public static TimeSpan RoundToNearest(this TimeSpan a, TimeSpan roundTo)
        {
            long ticks = (long)(Math.Round(a.Ticks / (double)roundTo.Ticks) * roundTo.Ticks);
            return new TimeSpan(ticks);
        }
        /// <summary>
        /// Converts <see cref="TimeSpan"/> objects to a simple human-readable string.  Examples: 3.1 seconds, 2 minutes, 4.23 hours, etc.
        /// </summary>
        /// <param name="span">The timespan.</param>
        /// <param name="significantDigits">Significant digits to use for output.</param>
        /// <returns></returns>
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
        /// Calculates the sum of the given timeSpans.
        /// </summary>
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
        /// Multiplies a given timeSpans.
        /// </summary>
        public static TimeSpan Multiply(this TimeSpan timeSpan, int multiplier)
        {
            return TimeSpan.FromTicks(timeSpan.Ticks * multiplier);
        }

    }
}
