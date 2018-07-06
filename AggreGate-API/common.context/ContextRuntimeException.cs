using System;
using JavaCompatibility;

namespace com.tibbo.aggregate.common.context
{
    public class ContextRuntimeException : RuntimeException
    {
        public ContextRuntimeException()
        {
        }

        public ContextRuntimeException(String message) : base(message)
        {
        }

        public ContextRuntimeException(Exception cause) : base(cause)
        {
        }

        public ContextRuntimeException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}