using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for working with queues.
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Dequeues a chunk of items from a <see cref="ConcurrentQueue{T}"/> with the specified chunk size.
        /// </summary>
        /// <typeparam name="T">The type of elements in the queue.</typeparam>
        /// <param name="queue">The concurrent queue from which to dequeue.</param>
        /// <param name="chunkSize">The number of items to dequeue in each chunk.</param>
        /// <returns>A list containing the dequeued items.</returns>
        public static List<T> DequeueChunk<T>(this ConcurrentQueue<T> queue, int chunkSize)
        {
            var results = new List<T>();
            for (int i = 0; i < chunkSize && queue.Count > 0; i++)
            {
                queue.TryDequeue(out var result);
                if (result != null)
                    results.Add(result);
            }
            return results;
        }

        /// <summary>
        /// Dequeues a chunk of items from a <see cref="Queue{T}"/> with the specified chunk size.
        /// </summary>
        /// <typeparam name="T">The type of elements in the queue.</typeparam>
        /// <param name="queue">The queue from which to dequeue.</param>
        /// <param name="chunkSize">The number of items to dequeue in each chunk.</param>
        /// <returns>An enumerable containing the dequeued items.</returns>
        public static IEnumerable<T> DequeueChunk<T>(this Queue<T> queue, int chunkSize)
        {
            var results = new List<T>();
            for (int i = 0; i < chunkSize && queue.Count > 0; i++)
            {
                queue.TryDequeue(out var result);
                if (result != null)
                    results.Add(result);
            }
            return results;
        }
    }
}
