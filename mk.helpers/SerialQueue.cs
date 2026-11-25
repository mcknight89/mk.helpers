using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Runs actions one at a time, in order, on its own background thread.
    /// </summary>
    public sealed class SerialQueue : IDisposable
    {
        private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();
        private readonly Thread _thread;
        private readonly HashSet<Delayed> _delayed = new HashSet<Delayed>();

        /// <summary>
        /// Raised on the queue's thread when an action throws. The queue keeps going.
        /// </summary>
        public event Action<Exception> Error;

        /// <summary>
        /// Starts the queue's thread, with the given name for debugging.
        /// </summary>
        public SerialQueue(string name)
        {
            _thread = new Thread(Run) { IsBackground = true, Name = name };
            _thread.Start();
        }

        private bool OnThread => Thread.CurrentThread == _thread;

        /// <summary>
        /// Queues an action and returns straight away. Ignored once the queue is disposed.
        /// </summary>
        public void Post(Action action)
        {
            if (_queue.IsAddingCompleted)
                return;

            try
            {
                _queue.Add(action);
            }
            catch (InvalidOperationException)
            {
                // disposed between the check and the add
            }
        }

        /// <summary>
        /// Runs an action on the queue and waits for it. Exceptions are thrown to the caller.
        /// </summary>
        public void Invoke(Action action)
        {
            Invoke(() =>
            {
                action();
                return true;
            });
        }

        /// <summary>
        /// Runs a function on the queue and waits for its result. Exceptions are thrown to the caller.
        /// </summary>
        public T Invoke<T>(Func<T> func)
        {
            if (OnThread || _queue.IsAddingCompleted)
                return func();

            var done = new TaskCompletionSource<T>();
            Post(() =>
            {
                try
                {
                    done.SetResult(func());
                }
                catch (Exception ex)
                {
                    done.SetException(ex);
                }
            });

            return done.Task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Queues an action after a delay. Dispose the result to cancel it.
        /// </summary>
        public IDisposable After(TimeSpan delay, Action action)
        {
            var delayed = new Delayed(this);
            lock (_delayed)
                _delayed.Add(delayed);

            delayed.Start(delay < TimeSpan.Zero ? TimeSpan.Zero : delay, action);
            return delayed;
        }

        /// <summary>
        /// Cancels pending After actions and lets queued ones finish.
        /// </summary>
        public void Dispose()
        {
            Delayed[] pending;
            lock (_delayed)
                pending = _delayed.ToArray();

            foreach (var x in pending)
                x.Dispose();

            _queue.CompleteAdding();
            if (!OnThread)
                _thread.Join(5000);
        }

        private void Run()
        {
            foreach (var action in _queue.GetConsumingEnumerable())
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Error?.Invoke(ex);
                }
            }
        }

        private sealed class Delayed : IDisposable
        {
            private readonly SerialQueue _owner;
            private Timer _timer;

            public Delayed(SerialQueue owner)
            {
                _owner = owner;
            }

            public void Start(TimeSpan delay, Action action)
            {
                _timer = new Timer(_ =>
                {
                    Dispose();
                    _owner.Post(action);
                }, null, delay, Timeout.InfiniteTimeSpan);
            }

            public void Dispose()
            {
                lock (_owner._delayed)
                    _owner._delayed.Remove(this);

                _timer?.Dispose();
            }
        }
    }
}
