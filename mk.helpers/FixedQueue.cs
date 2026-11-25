using System;
using System.Collections;
using System.Collections.Generic;

namespace mk.helpers
{
    /// <summary>
    /// A queue that drops the oldest item when it's full.
    /// </summary>
    public class FixedQueue<T> : IReadOnlyCollection<T>
    {
        private readonly Queue<T> _items;

        /// <summary>
        /// Creates a queue that holds at most capacity items.
        /// </summary>
        public FixedQueue(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be at least 1");

            Capacity = capacity;
            _items = new Queue<T>(capacity);
        }

        /// <summary>
        /// The most items the queue holds.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// How many items the queue holds now.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Adds an item, dropping the oldest when full.
        /// </summary>
        public void Enqueue(T item)
        {
            _items.Enqueue(item);
            while (_items.Count > Capacity)
                _items.Dequeue();
        }

        /// <summary>
        /// Removes every item.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Goes through the items, oldest first.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
