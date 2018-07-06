using System;

namespace com.tibbo.aggregate.common.context
{
    public class ContextException : AggreGateException
    {
        public ContextException(String message) : base(message)
        {
        }

        public ContextException(Exception cause) : base(cause)
        {
        }

        public ContextException(String message, Exception cause) : base(message, cause)
        {
        }

        public ContextException(String message, Exception cause, String details) : base(message, cause, details)
        {
        }
    }
}