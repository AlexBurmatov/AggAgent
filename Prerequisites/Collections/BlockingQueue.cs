using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Collections
{
    public class BlockingQueue<T> : IBlockingQueue<T>, IEnumerable<T>
    {
        private readonly Queue<T> queue;

        private readonly Object locker = new Object();

        public BlockingQueue() : this(new Queue<T>())
        {
        }

        public BlockingQueue(Queue<T> q)
        {
            queue = q;
        }

        private T basicDequeue()
        {
            return queue.Dequeue();
        }

        public int size
        {
            get { lock (locker) return queue.Count; }
        }

        public void clear()
        {
            lock (locker) queue.Clear();
        }

        public void enqueue(T item)
        {
            lock (locker)
            {
                queue.Enqueue(item);
                Monitor.Pulse(locker);
            }
        }

        public T dequeue()
        {
            lock (locker)
            {
                var result = basicDequeue();
                Monitor.Pulse(locker);
                return result;
            }
        }

        public void pulse()
        {
            lock (locker) Monitor.Pulse(locker);
        }

        public void pulseAll()
        {
            lock (locker) Monitor.PulseAll(locker);
        }

        public bool isEmpty()
        {
            return queue.Count == 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}