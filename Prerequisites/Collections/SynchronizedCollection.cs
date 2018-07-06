using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    public class SynchronizedCollection<T> : ICollection<T>, ICloneable
    {
        protected ICollection<T> coll;

        public SynchronizedCollection(ICollection<T> coll)
        {
            if (coll == null)
            {
                throw new ArgumentNullException("coll");
            }
            this.coll = coll;
        }

        public void Add(T item)
        {
            lock (this)
            {
                coll.Add(item);
            }
        }

        public void Clear()
        {
            lock (this)
            {
                coll.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this)
            {
                return coll.Contains(item);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            lock (this)
            {
                coll.CopyTo(array, index);
            }
        }

        public bool Remove(T item)
        {
            lock (this)
            {
                return coll.Remove(item);
            }
        }

        public int Count
        {
            get
            {
                lock (this)
                {
                    return coll.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock (this)
                {
                    return coll.IsReadOnly;
                }
            }
        }

        public bool isSynchronized
        {
            get { return true; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this)
            {
                return new SynchronizedIterator<T>(coll, coll.GetEnumerator());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                return new SynchronizedIterator<T>(coll, coll.GetEnumerator());
            }
        }

        public virtual Object Clone()
        {
            lock (this)
            {
                if (coll is ICloneable)
                {
                    return new SynchronizedCollection<T>
                        ((ICollection<T>) (((ICloneable) coll).Clone()));
                }
                throw new InvalidOperationException("Invalid_NotCloneable");
            }
        }

        private sealed class SynchronizedIterator<Tt> : IEnumerator<Tt>
        {
            private ICollection<Tt> coll;
            private IEnumerator<Tt> iterator;

            public SynchronizedIterator(ICollection<Tt> coll, IEnumerator<Tt> iterator)
            {
                this.coll = coll;
                this.iterator = iterator;
            }

            bool IEnumerator.MoveNext()
            {
                lock (this)
                {
                    return iterator.MoveNext();
                }
            }

            void IEnumerator.Reset()
            {
                lock (coll)
                {
                    iterator.Reset();
                }
            }

            Tt IEnumerator<Tt>.Current
            {
                get
                {
                    lock (coll)
                    {
                        return iterator.Current;
                    }
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    lock (coll)
                    {
                        return iterator.Current;
                    }
                }
            }

            public void Dispose()
            {
            }
        } ; 
    } ;
}