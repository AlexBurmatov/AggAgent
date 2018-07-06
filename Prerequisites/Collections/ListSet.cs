using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    public sealed class ListSet<T> : ISet<T>
    {
        private readonly IList<T> list;

        public ListSet()
        {
            list = new List<T>();
        }

        public ListSet(IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            this.list = list;
        }

        public void Add(T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T value)
        {
            return list.Contains(value);
        }

        public bool Remove(T value)
        {
            return list.Remove(value);
        }

        public void AddAll(IEnumerable<T> aCollection)
        {
            foreach (var each in aCollection)
            {
                Add(each);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            list.CopyTo(array, index);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return list.IsReadOnly; }
        }

        public bool isSynchronized
        {
            get { return false; }
        }

        public IEnumerator<T> getIterator()
        {
            return list.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } ;
}