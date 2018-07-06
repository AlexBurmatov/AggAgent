using System;
using System.Collections.Generic;
using OX.Copyable;

namespace com.tibbo.aggregate.common.util
{
    public class AgDictionary<K, V>: Dictionary<K, V>
    {
        public AgDictionary()
        {
        }

        protected AgDictionary(int size) : base(size)
        {
        }

        public AgDictionary(AgDictionary<K, V> selectionValues) : base(selectionValues)
        {
        }

        public override int GetHashCode()
        {
            int hashCode = 1;
            Enumerator e = GetEnumerator();
            while (e.MoveNext())
            {
                KeyValuePair<K, V> pair = e.Current;
                
                hashCode = 31 * hashCode + pair.GetHashCode();
            }

            return hashCode;
        }
    }
}