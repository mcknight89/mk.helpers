using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mk.helpers
{
    /// <summary>
    /// A performance timer that measures the elapsed time for different actions and intervals.
    /// </summary>
    public class PerfTimer
    {
        // Private fields
        private readonly Stopwatch _timer = new Stopwatch(); // Stopwatch to measure time
        private bool _enabled = true; // Flag to enable or disable the timer

        private List<TimeSpan> times = new List<TimeSpan>(); // List to store individual action times

        private object _lock = new object(); // Object used for thread safety

        /// <summary>
        /// Initializes a new instance of the PerfTimer class with default settings (enabled).
        /// </summary>
        public PerfTimer()
        {
            // No additional initialization needed
        }

        /// <summary>
        /// Initializes a new instance of the PerfTimer class with the specified enabled status.
        /// </summary>
        /// <param name="enabled">True to enable the timer, false to disable it.</param>
        public PerfTimer(bool enabled)
        {
            _enabled = enabled;
        }

        /// <summary>
        /// Starts the timer to measure the elapsed time for an action.
        /// </summary>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer Start()
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                    _timer.Start();
                return this;
            }
        }

        /// <summary>
        /// Stops the timer and records the elapsed time for the completed action.
        /// </summary>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer Stop()
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                {
                    _timer.Stop();
                    times.Add(_timer.Elapsed);
                }
                return this;
            }
        }

        /// <summary>
        /// Resets the timer to measure the elapsed time for the next action.
        /// </summary>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer Reset()
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                    _timer.Reset();
                return this;
            }
        }

        /// <summary>
        /// Stops the timer, records the elapsed time, and starts the timer again.
        /// Used to measure intervals between actions.
        /// </summary>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer Interval()
        {
            lock (_lock) // Thread-safe operation using lock
            {
                Stop();
                Reset();
                Start();
                return this;
            }
        }

        /// <summary>
        /// Executes the specified action and records the elapsed time if the timer is enabled.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer Execute(Action action)
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                    action();
                return this;
            }
        }

        /// <summary>
        /// Gets a collection of all recorded action times.
        /// </summary>
        public ICollection<TimeSpan> AllTimes { get { return times; } }

        /// <summary>
        /// Gets the sum of all recorded action times.
        /// </summary>
        public TimeSpan AllTimesElapsed { get { return times.Sum(); } }

        /// <summary>
        /// Gets the elapsed time for the current action (since Start() was called).
        /// </summary>
        public TimeSpan Elapsed { get { return _timer.Elapsed; } }
    }
}
