using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections
{
    public class SynchronizedDictionary<KeyT, ValueT> : SynchronizedCollection<KeyValuePair<KeyT, ValueT>>,
                                                        IDictionary<KeyT, ValueT>
    {
        protected IDictionary<KeyT, ValueT> dict;

        public SynchronizedDictionary(IDictionary<KeyT, ValueT> dict) : base(dict)
        {
            this.dict = dict;
        }

        public bool ContainsKey(KeyT key)
        {
            lock (this)
            {
                return dict.ContainsKey(key);
            }
        }

        void IDictionary<KeyT, ValueT>.Add(KeyT key, ValueT value)
        {
            lock (this)
            {
                dict.Add(key, value);
            }
        }

        void ICollection<KeyValuePair<KeyT, ValueT>>.Clear()
        {
            lock (this)
            {
                dict.Clear();
            }
        }

        bool ICollection<KeyValuePair<KeyT, ValueT>>.Contains(KeyValuePair<KeyT, ValueT> key)
        {
            lock (this)
            {
                return dict.Contains(key);
            }
        }

        IEnumerator<KeyValuePair<KeyT, ValueT>> IEnumerable<KeyValuePair<KeyT, ValueT>>.GetEnumerator()
        {
            lock (this)
            {
                return new SynchronizedDictionaryEnumerator<KeyT, ValueT>(dict, dict.GetEnumerator());
            }
        }

        public bool Remove(KeyT key)
        {
            lock (this)
            {
                return dict.Remove(key);
            }
        }

        public bool TryGetValue(KeyT key, out ValueT value)
        {
            lock (this)
            {
                return dict.TryGetValue(key, out value);
            }
        }

        public ValueT this[KeyT key]
        {
            get
            {
                lock (this)
                {
                    return dict[key];
                }
            }
            set
            {
                lock (this)
                {
                    dict[key] = value;
                }
            }
        }

        public ICollection<KeyT> Keys
        {
            get
            {
                lock (this)
                {
                    return new SynchronizedCollection<KeyT>(dict.Keys);
                }
            }
        }

        public ICollection<ValueT> Values
        {
            get
            {
                lock (this)
                {
                    return new SynchronizedCollection<ValueT>(dict.Values);
                }
            }
        }

        public override Object Clone()
        {
            lock (this)
            {
                if (dict is ICloneable)
                {
                    return new SynchronizedDictionary<KeyT, ValueT>
                        ((IDictionary<KeyT, ValueT>) (((ICloneable) dict).Clone()));
                }
                throw new InvalidOperationException("Invalid_NotCloneable");
            }
        }

        private sealed class SynchronizedDictionaryEnumerator<KeyTt, ValueTt> : IEnumerator<KeyValuePair<KeyT, ValueT>>
        {
            private IDictionary<KeyTt, ValueTt> dict;
            private IEnumerator<KeyValuePair<KeyT, ValueT>> iterator;

            public SynchronizedDictionaryEnumerator(IDictionary<KeyTt, ValueTt> dict,
                                                    IEnumerator<KeyValuePair<KeyT, ValueT>> iterator)
            {
                this.dict = dict;
                this.iterator = iterator;
            }

            bool IEnumerator.MoveNext()
            {
                lock (dict)
                {
                    return iterator.MoveNext();
                }
            }

            void IEnumerator.Reset()
            {
                lock (dict)
                {
                    iterator.Reset();
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public KeyValuePair<KeyT, ValueT> Current
            {
                get
                {
                    lock (dict)
                    {
                        return iterator.Current;
                    }
                }
            }

            public void Dispose()
            {
            }
        }
    }
}