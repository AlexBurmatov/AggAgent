using System;

namespace JavaCompatibility
{
    public class RuntimeException : Exception
    {
        public RuntimeException()
        {
        }

        protected RuntimeException(string message)
            : base(message)
        {
        }

        public RuntimeException(Exception cause) : base("", cause)
        {
        }

        public RuntimeException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}