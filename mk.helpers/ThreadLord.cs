using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Process a collection of tasks as fast as possible using a thread pool.
    /// </summary>
    /// <typeparam name="T">The type of tasks to process.</typeparam>
    public class ThreadLord<T> : IDisposable
    {
        private int queueLimit;
        private readonly int maxThreads;

        private long processed;
        private long failed;
        private int lastSecondProcessed;
        private int lastSecondCount;
        private readonly int[] avgData = new int[10];

        private ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim itemSignal = new SemaphoreSlim(0);
        private SemaphoreSlim slots;

        private readonly Action<T> workerCallback;

        private Task[] workers;
        private CancellationTokenSource cts;
        private Task statsTask;

        private volatile LordState _state = LordState.Stopped;

        private int enqueuedCount;
        private int currentlyProcessing;

        // on error
        private Action<Exception> onError = null;

        /// <summary>
        /// Gets the current state of the ThreadLord instance.
        /// </summary>
        public LordState State => _state;

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
            workerCallback = onWork ?? throw new ArgumentNullException(nameof(onWork));
        }

        /// <summary>
        /// Initializes a new instance of the ThreadLord class with a specified maximum thread count.
        /// </summary>
        /// <param name="onWork">The callback action for processing tasks.</param>
        /// <param name="maxThreads">The maximum number of worker threads.</param>
        public ThreadLord(Action<T> onWork, int maxThreads)
        {
            if (maxThreads <= 0) throw new ArgumentOutOfRangeException(nameof(maxThreads));
            this.maxThreads = maxThreads;
            workers = new Task[maxThreads];
            workerCallback = onWork ?? throw new ArgumentNullException(nameof(onWork));
        }

        private async Task StatsWorkerAsync(CancellationToken token)
        {
#if NET6_0_OR_GREATER
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            var idx = 0;
            try
            {
                while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
                {
                    lastSecondCount = Interlocked.Exchange(ref lastSecondProcessed, 0);
                    avgData[idx] = lastSecondCount;
                    idx = (idx + 1) % avgData.Length;
                }
            }
            catch (OperationCanceledException) { }
            finally { timer.Dispose(); }
#else
            var idx = 0;
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000, token).ConfigureAwait(false);
                lastSecondCount = Interlocked.Exchange(ref lastSecondProcessed, 0);
                avgData[idx] = lastSecondCount;
                idx = (idx + 1) % avgData.Length;
            }
#endif
        }

        private async Task WorkerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await itemSignal.WaitAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { break; }

                if (queue.TryDequeue(out var work))
                {
                    Interlocked.Decrement(ref enqueuedCount);
                    Interlocked.Increment(ref currentlyProcessing);
                    try
                    {
                        try
                        {
                            workerCallback(work);
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Increment(ref failed);
                            onError?.Invoke(ex);
                        }
                    }
                    finally
                    {
                        // count attempts (success or failure)
                        Interlocked.Increment(ref processed);
                        Interlocked.Increment(ref lastSecondProcessed);
                        Interlocked.Decrement(ref currentlyProcessing);
                        slots?.Release();
                    }
                }
            }
        }

        /// <summary>
        /// On error callback.
        /// </summary>
        // <param name="onError">The action to execute when an error occurs.</param>
        public ThreadLord<T> OnError(Action<Exception> onError)
        {
            this.onError = onError;
            return this;
        }

        /// <summary>
        /// Enqueues a task to be processed.
        /// </summary>
        /// <param name="data">The task data to enqueue.</param>
        public void Enqueue(T data)
        {
            if (data == null) return;
            if (_state == LordState.Stopped) throw new InvalidOperationException("ThreadLord is not running.");

            slots?.Wait();
            queue.Enqueue(data);
            Interlocked.Increment(ref enqueuedCount);
            itemSignal.Release();
        }

        /// <summary>
        /// Clears all tasks from the queue.
        /// </summary>
        public void Clear()
        {
            while (queue.TryDequeue(out _))
            {
                Interlocked.Decrement(ref enqueuedCount);
                slots?.Release();
            }
        }

        /// <summary>
        /// Sets the maximum size of the task queue.
        /// </summary>
        /// <param name="limit">The maximum number of tasks to be queued.</param>
        /// <returns>The ThreadLord instance with the queue size limit set.</returns>
        public ThreadLord<T> LimitQueue(int limit)
        {
            if (_state == LordState.Running) throw new InvalidOperationException("Set queue limit before Start.");
            queueLimit = limit;
            slots = queueLimit > 0 ? new SemaphoreSlim(queueLimit, queueLimit) : null;
            return this;
        }

        /// <summary>
        /// Starts processing tasks using worker threads.
        /// </summary>
        /// <returns>The ThreadLord instance in the Running state.</returns>
        public ThreadLord<T> Start()
        {
            if (_state != LordState.Stopped) return this;

            _state = LordState.Running;
            cts = new CancellationTokenSource();

            for (int i = 0; i < maxThreads; i++)
            {
                workers[i] = Task.Factory.StartNew(
                    async obj => await WorkerAsync((CancellationToken)obj).ConfigureAwait(false),
                    cts.Token,
                    CancellationToken.None,
                    TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.Default).Unwrap();
            }

            statsTask = Task.Run(() => StatsWorkerAsync(cts.Token), cts.Token);
            return this;
        }

        /// <summary>
        /// Stops processing tasks.
        /// </summary>
        /// <returns>The ThreadLord instance in the Stopping state.</returns>
        public ThreadLord<T> Stop()
        {
            if (_state == LordState.Stopped) return this;
            _state = LordState.Stopping;
            cts?.Cancel();
            return this;
        }

        /// <summary>
        /// Gets the number of tasks currently enqueued.
        /// </summary>
        public long Enqueued => Volatile.Read(ref enqueuedCount);

        /// <summary>
        /// Gets the number of worker threads currently active.
        /// </summary>
        public long Workers
        {
            get
            {
                var active = 0;
                foreach (var t in workers)
                {
                    if (t != null && !t.IsCompleted) active++;
                }
                return active;
            }
        }

        /// <summary>
        /// Gets the count of tasks processed in the last second.
        /// </summary>
        public long ProcessedLastSecond => lastSecondCount == 0 ? lastSecondProcessed : lastSecondCount;

        /// <summary>
        /// Gets the average number of tasks processed per second.
        /// </summary>
        public long ProcessedPerSecondAverage
        {
            get
            {
                int sum = 0, count = 0;
                foreach (var v in avgData)
                {
                    if (v != 0) { sum += v; count++; }
                }
                return count == 0 ? lastSecondProcessed : sum / count;
            }
        }

        /// <summary>
        /// Gets the queue size limit.
        /// </summary>
        public long QueueLimit => queueLimit;

        /// <summary>
        /// Gets the total number of processed tasks.
        /// </summary>
        public long Processed => Volatile.Read(ref processed);

        /// <summary>
        /// Gets the number of worker threads currently processing tasks.
        /// </summary>
        public long CurrentlyProcessing => Volatile.Read(ref currentlyProcessing);

        /// <summary>
        /// Gets the total number of failed tasks.
        /// </summary>
        public long Failed => Volatile.Read(ref failed);

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
            while (Enqueued > 0 || CurrentlyProcessing > 0)
            {
                whileWaiting?.Invoke();
                Thread.SpinWait(200);
            }
            return this;
        }

        /// <summary>
        /// Disposes of the ThreadLord instance and stops processing.
        /// </summary>
        public void Dispose()
        {
            Stop();
            try { Task.WhenAll(workers ?? Array.Empty<Task>()).Wait(TimeSpan.FromSeconds(5)); } catch { }
            try { statsTask?.Wait(TimeSpan.FromSeconds(1)); } catch { }

            cts?.Dispose();
            slots?.Dispose();
            itemSignal?.Dispose();

            queue = null;
            _state = LordState.Stopped;
        }
    }
}
