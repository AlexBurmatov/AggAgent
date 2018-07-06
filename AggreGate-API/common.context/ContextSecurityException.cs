using System;

namespace com.tibbo.aggregate.common.context
{
    public class ContextSecurityException : ContextException
    {
        public ContextSecurityException(String message) : base(message)
        {
        }

        public ContextSecurityException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}