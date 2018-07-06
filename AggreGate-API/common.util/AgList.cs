using System;
using System.Collections.Generic;
using OX.Copyable;

namespace com.tibbo.aggregate.common.util
{
    using System.Linq;

    using com.tibbo.aggregate.common.datatable;

    public class AgList<T>: List<T>
    {

        public override int GetHashCode()
        {
            int hashCode = 1;
            List<T>.Enumerator e = GetEnumerator();
            while (e.MoveNext())
            {
                T value = e.Current;
                hashCode = 31 * hashCode + (value == null ? 0 : value.GetHashCode());
            }

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            var other = (AgList<T>)obj;
            return this.SequenceEqual(other);
        }
    }
}