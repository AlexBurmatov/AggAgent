using System;
using System.Collections.Generic;

namespace Collections
{
    public class SynchronizedList<T> : SynchronizedCollection<T>, IList<T>
    {
        protected IList<T> list;

        public SynchronizedList(IList<T> list)
            : base(list)
        {
            this.list = list;
        }

        public SynchronizedList() : base(new List<T>())
        {
        }

        public int IndexOf(T value)
        {
            lock (this)
            {
                return list.IndexOf(value);
            }
        }

        public void Insert(int index, T value)
        {
            lock (this)
            {
                list.Insert(index, value);
            }
        }

        public void RemoveAt(int index)
        {
            lock (this)
            {
                list.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (this)
                {
                    return list[index];
                }
            }
            set
            {
                lock (this)
                {
                    list[index] = value;
                }
            }
        }

        public override Object Clone()
        {
            lock (this)
            {
                if (list is ICloneable)
                {
                    return new SynchronizedList<T>((IList<T>) (((ICloneable) list).Clone()));
                }
                throw new InvalidOperationException("Invalid_NotCloneable");
            }
        }

        public bool isEmpty()
        {
            return Count == 0;
        }
    } ;
}