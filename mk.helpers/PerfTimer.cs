using System;
using System.Collections.Concurrent;
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

        private ConcurrentDictionary<string, TimeSpan> checkpoints = new ConcurrentDictionary<string, TimeSpan>(); // Dictionary to store named action times

        private List<string> _checkpointOrder = new List<string>(); // Preserve insertion order of checkpoints

        private object _lock = new object(); // Object used for thread safety

        /// <summary>
        /// Initializes a new instance of the PerfTimer class with default settings (enabled).
        /// </summary>
        public PerfTimer()
        {
            // No additional initialization needed
        }

        /// <summary>
        /// Create new instance of PerfTimer and start it
        /// </summary>
        public static PerfTimer StartNew()
        {
            return new PerfTimer().Start();
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
        /// Used to measure intervals between actions. This does not clear checkpoints.
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
        /// Executes the specified action and records the elapsed time in a checkpoint. the timer is not effected and continues to run / keep its current state.
        /// </summary>
        /// <param name="name">name of the checkpoint</param>
        /// <param name="action">The action to be executed.</param>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer ExecuteWithCheckpoint(string name, Action action)
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                {
                    var start = DateTime.Now;
                    try
                    {
                        action();
                    }
                    finally
                    {
                        AddCheckpoint(name, DateTime.Now - start);
                    }
                }
                return this;
            }
        }

        /// <summary>
        /// Add new checkpoint (store name, time). Timer is not reset!
        /// </summary>
        /// <param name="name">name of the checkpoint</param>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer AddCheckpoint(string name)
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                    return AddCheckpoint(name, _timer.Elapsed);
                return this;
            }
        }

        /// <summary>
        /// Add new checkpoint (store name, time). timer is not effected.
        /// </summary>
        /// <param name="name">name of the checkpoint</param>
        /// <param name="timeSpan">timespan to store</param>
        /// <returns>The PerfTimer instance for method chaining.</returns>
        public PerfTimer AddCheckpoint(string name, TimeSpan timeSpan)
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                {
                    if (checkpoints.TryAdd(name, timeSpan))
                        _checkpointOrder.Add(name);
                    else
                        checkpoints[name] = timeSpan;
                }
                return this;
            }
        }

        /// <summary>
        /// Get the timespan of a saved checkpoint, default is TimeSpan.Zero
        /// </summary>
        /// <param name="name">name of the checkpoint</param>
        public TimeSpan GetCheckpoint(string name)
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled && checkpoints.TryGetValue(name, out var ts))
                    return ts;
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Removes a checkpoint
        /// </summary>
        /// <param name="name">name of the checkpoint</param>
        public PerfTimer RemoveCheckpoint(string name)
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                {
                    if (checkpoints.TryRemove(name, out _))
                        _checkpointOrder.Remove(name);
                }
                return this;
            }
        }

        /// <summary>
        /// Clears all recorded checkpoints.
        /// </summary>
        public PerfTimer ClearCheckpoints()
        {
            lock (_lock) // Thread-safe operation using lock
            {
                if (_enabled)
                {
                    checkpoints.Clear();
                    _checkpointOrder.Clear();
                }
                return this;
            }
        }

        /// <summary>
        /// Gets a collection of all recorded checkpoints.
        /// </summary>
        public IDictionary<string, TimeSpan> GetCheckpoints()
        {
            lock (_lock) // Thread-safe operation using lock
            {
                return _checkpointOrder
                    .Where(name => checkpoints.ContainsKey(name))
                    .ToDictionary(name => name, name => checkpoints[name]);
            }
        }

        /// <summary>
        /// Clears all recorded action times, checkpoints will remain the same.
        /// </summary>
        public void Clear()
        {
            lock (_lock) // Thread-safe operation using lock
            {
                times.Clear();
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
