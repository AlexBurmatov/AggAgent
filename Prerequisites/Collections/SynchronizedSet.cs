using System;
using System.Collections.Generic;

namespace Collections
{
    public class SynchronizedSet<T> : SynchronizedCollection<T>, ISet<T>
    {
        protected ISet<T> set;

        public SynchronizedSet(ISet<T> set) : base(set)
        {
            this.set = set;
        }

        void ICollection<T>.Add(T value)
        {
            lock (this)
            {
                set.Add(value);
            }
        }

        void ICollection<T>.Clear()
        {
            lock (this)
            {
                set.Clear();
            }
        }

        bool ICollection<T>.Contains(T value)
        {
            lock (this)
            {
                return set.Contains(value);
            }
        }

        bool ICollection<T>.Remove(T value)
        {
            lock (this)
            {
                return set.Remove(value);
            }
        }

        void ISet<T>.AddAll(IEnumerable<T> aCollection)
        {
            foreach (var each in aCollection)
            {
                Add(each);
            }
        }

        public override Object Clone()
        {
            lock (this)
            {
                if (set is ICloneable)
                {
                    return new SynchronizedSet<T>
                        ((ISet<T>) (((ICloneable) set).Clone()));
                }
                throw new InvalidOperationException("Invalid_NotCloneable");
            }
        }
    }
}