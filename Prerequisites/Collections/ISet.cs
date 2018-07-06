using System.Collections.Generic;

namespace Collections
{
    public interface ISet<T> : ICollection<T>
    {
        void AddAll(IEnumerable<T> aCollection);
    } ;
}