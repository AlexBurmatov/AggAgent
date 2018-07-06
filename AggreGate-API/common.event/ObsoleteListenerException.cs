using System;

namespace com.tibbo.aggregate.common.@event
{
    public class ObsoleteListenerException : EventHandlingException
    {
        public ObsoleteListenerException(string message) : base(message)
        {
        }

        public ObsoleteListenerException(Exception cause) : base(cause)
        {
        }

        public ObsoleteListenerException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}