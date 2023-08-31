using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Process a collection of tasks as fast as posiable using a thread pool.
    /// </summary>
    /// <typeparam name="T">The type of tasks to process.</typeparam>
    public class ThreadLord<T> : IDisposable
    {
        private int queueLimit = 0;
        private int maxThreads = 0;

        private long processed = 0;
        private int lastSecondProcessed = 0;
        private int lastSecondCount = 0;
        private int[] avgData = new int[10];

        private ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private Action<T> workerCallback;

        private Task[] workers = new Task[10];
        private Task manager;

        private LordState _state = LordState.Stopped;

        /// <summary>
        /// Gets the current state of the ThreadLord instance.
        /// </summary>
        public LordState State { get { return _state; } }

        /// <summary>
        /// Represents the state of the ThreadLord instance.
        /// </summary>
        public enum LordState
        {
            Stopped,
            Running,
            Stopping
        }

        /// <summary>
        /// Initializes a new instance of the ThreadLord class with a default maximum thread count.
        /// </summary>
        /// <param name="onWork">The callback action for processing tasks.</param>
        public ThreadLord(Action<T> onWork)
        {
            maxThreads = Environment.ProcessorCount * 4;
            workers = new Task[maxThreads];
            workerCallback = onWork;
        }

        /// <summary>
        /// Initializes a new instance of the ThreadLord class with a specified maximum thread count.
        /// </summary>
        /// <param name="onWork">The callback action for processing tasks.</param>
        /// <param name="maxThreads">The maximum number of worker threads.</param>
        public ThreadLord(Action<T> onWork, int maxThreads)
        {
            this.maxThreads = maxThreads;
            workers = new Task[maxThreads];
            workerCallback = onWork;
        }

        private void ManagerWorker()
        {
            var dt = DateTime.Now;
            var lastSecondIndic = 0;

            _state = LordState.Running;

            while (_state == LordState.Running)
            {
                // Data stats
                if (DateTime.Now > dt.AddSeconds(1))
                {
                    dt = DateTime.Now;
                    Interlocked.Exchange(ref lastSecondCount, lastSecondProcessed);
                    Interlocked.Exchange(ref lastSecondProcessed, 0);

                    if (lastSecondIndic > avgData.Length - 1)
                        lastSecondIndic = 0;
                    avgData[lastSecondIndic] = lastSecondCount;
                    lastSecondIndic++;
                }

                // While any threads free for work
                if (workers.Any(d => d == null || d.Status == TaskStatus.RanToCompletion))
                {
                    // Find free indic
                    var indic = workers.Select((s, i) => new { s, i })
                        .Where(d => d.s == null || d.s.Status == TaskStatus.RanToCompletion)
                        .Select(d => d.i).First();
                    // Add work
                    workers[indic] = Task.Factory.StartNew(Worker);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            _state = LordState.Stopped;
        }

        private void Worker()
        {
            while (Enqueued > 0)
            {
                if (_state != LordState.Running)
                    throw new InvalidOperationException("cancellation requested");
                if (queue.TryDequeue(out var work))
                {
                    workerCallback.Invoke(work);
                    Interlocked.Increment(ref processed);
                    Interlocked.Increment(ref lastSecondProcessed);
                }
                else
                {
                      Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// Enqueues a task to be processed.
        /// </summary>
        /// <param name="data">The task data to enqueue.</param>
        public void Enqueue(T data)
        {
            while (queueLimit != 0 && queue.Count > queueLimit)
                Thread.Sleep(10);
            if (data != null)
                queue.Enqueue(data);
        }

        /// <summary>
        /// Clears all tasks from the queue.
        /// </summary>
        public void Clear()
        {
            queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Sets the maximum size of the task queue.
        /// </summary>
        /// <param name="limit">The maximum number of tasks to be queued.</param>
        /// <returns>The ThreadLord instance with the queue size limit set.</returns>
        public ThreadLord<T> LimitQueue(int limit)
        {
            queueLimit = limit;
            return this;
        }

        /// <summary>
        /// Starts processing tasks using the specified number of worker threads.
        /// </summary>
        /// <returns>The ThreadLord instance in the Running state.</returns>
        public ThreadLord<T> Start()
        {
            if (_state == LordState.Stopped)
            {
                manager = Task.Factory.StartNew(ManagerWorker);
            }
            return this;
        }

        /// <summary>
        /// Stops processing tasks.
        /// </summary>
        /// <returns>The ThreadLord instance in the Stopping state.</returns>
        public ThreadLord<T> Stop()
        {
            _state = LordState.Stopping;
            return this;
        }

        /// <summary>
        /// Gets the number of tasks currently enqueued.
        /// </summary>
        public long Enqueued
        {
            get
            {
                return queue.Count();
            }
        }

        /// <summary>
        /// Gets the number of worker threads currently active.
        /// </summary>
        public long Workers
        {
            get
            {
                return maxThreads - workers.Count(d => d == null || d.Status == TaskStatus.RanToCompletion);
            }
        }

        /// <summary>
        /// Gets the count of tasks processed in the last second.
        /// </summary>
        public long ProcessedLastSecond
        {
            get
            {
                return lastSecondCount == 0 ? lastSecondProcessed : lastSecondCount;
            }
        }

        /// <summary>
        /// Gets the average number of tasks processed per second.
        /// </summary>
        public long ProcessedPerSecondAverage
        {
            get
            {
                var data = avgData.Where(d => d != 0);
                if (!data.Any())
                    return lastSecondProcessed;
                return data.Sum() / data.Count();
            }
        }

        /// <summary>
        /// Gets the queue size limit.
        /// </summary>
        public long QueueLimit
        {
            get
            {
                return queueLimit;
            }
        }

        /// <summary>
        /// Gets the total number of processed tasks.
        /// </summary>
        public long Processed
        {
            get
            {
                return processed;
            }
        }

        /// <summary>
        /// Gets the number of worker threads currently processing tasks.
        /// </summary>
        public long CurrentlyProcessing
        {
            get
            {
                return workers.Count() - workers.Select((s, i) => new { s, i })
                        .Where(d => d.s == null || d.s.Status == TaskStatus.RanToCompletion)
                        .Select(d => d.i).Count();
            }
        }

        /// <summary>
        /// Waits for all tasks to be processed and then stops the processing.
        /// </summary>
        /// <param name="whileWaiting">An optional action to perform while waiting for tasks to complete.</param>
        /// <returns>The ThreadLord instance after all tasks are processed and processing is stopped.</returns>
        public ThreadLord<T> WaitAllAndStop(Action whileWaiting = null)
        {
            WaitAll(whileWaiting);
            Stop();
            return this;
        }

        /// <summary>
        /// Waits for all tasks to be processed.
        /// </summary>
        /// <param name="whileWaiting">An optional action to perform while waiting for tasks to complete.</param>
        /// <returns>The ThreadLord instance after all tasks are processed.</returns>
        public ThreadLord<T> WaitAll(Action whileWaiting = null)
        {
            while (!workers.All(d => d == null || d.Status == TaskStatus.RanToCompletion) || Enqueued > 0)
            {
                whileWaiting?.Invoke();
                Thread.Sleep(25);
            }
            return this;
        }

        /// <summary>
        /// Disposes of the ThreadLord instance and stops processing.
        /// </summary>
        public void Dispose()
        {
            Stop();
            queue = null;
        }
    }
}
