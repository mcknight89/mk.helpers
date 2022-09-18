using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers
{
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
        public LordState State { get { return _state; } }


        public enum LordState
        {
            Stopped,
            Running,
            Stopping
        }

        public ThreadLord(Action<T> onWork)
        {
            maxThreads = Environment.ProcessorCount * 4;
            workers = new Task[maxThreads];
            workerCallback = onWork;
        }
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

                queue.TryDequeue(out var work);
                if (work != null)
                {
                    workerCallback.Invoke(work);
                    Interlocked.Increment(ref processed);
                    Interlocked.Increment(ref lastSecondProcessed);
                }
            }
        }


        public void Enqueue(T data)
        {
            while (queueLimit != 0 && queue.Count > queueLimit)
                Thread.Sleep(10);
            if (data != null)
                queue.Enqueue(data);
        }


        public void Clear()
        {
            queue = new ConcurrentQueue<T>();
        }

        public ThreadLord<T> LimitQueue(int limit)
        {
            queueLimit = limit;
            return this;
        }

        public ThreadLord<T> Start()
        {
            if (_state == LordState.Stopped)
            {
                manager = Task.Factory.StartNew(ManagerWorker);

            }
            return this;
        }

        public ThreadLord<T> Stop()
        {
            _state = LordState.Stopping;
            return this;
        }

        public long Enqueued
        {
            get
            {
                return queue.Count();
            }
        }

        public long Workers
        {
            get
            {
                return maxThreads - workers.Count(d => d == null || d.Status == TaskStatus.RanToCompletion);
            }
        }


        public long ProcessedLastSecond
        {
            get
            {
                return lastSecondCount == 0 ? lastSecondProcessed : lastSecondCount;
            }
        }

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
        public long QueueLimit
        {
            get
            {
                return queueLimit;
            }
        }
        public long Processed
        {
            get
            {
                return processed;
            }
        }
        public long CurrentlyProcessing
        {
            get
            {
                return workers.Count() - workers.Select((s, i) => new { s, i })
                        .Where(d => d.s == null || d.s.Status == TaskStatus.RanToCompletion)
                        .Select(d => d.i).Count();
            }
        }

        public ThreadLord<T> WaitAllAndStop(Action whileWaiting = null)
        {
            WaitAll(whileWaiting);
            Stop();
            return this;
        }
        public ThreadLord<T> WaitAll(Action whileWaiting = null)
        {
            while (!workers.All(d => d == null || d.Status == TaskStatus.RanToCompletion) || Enqueued > 0)
            {
                whileWaiting?.Invoke();
                Thread.Sleep(25);
            }
            return this;
        }

        public void Dispose()
        {
            Stop();
            queue = null;
        }
    }
}
