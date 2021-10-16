using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace mk.helpers
{
    public static class QueueExtensions
    {


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
