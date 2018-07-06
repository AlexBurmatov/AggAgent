using System;

namespace JavaCompatibility
{
    public class CloneNotSupportedException : Exception
    {
        public CloneNotSupportedException()
        {
        }

        public CloneNotSupportedException(String s) : base(s)
        {
        }
    }
}