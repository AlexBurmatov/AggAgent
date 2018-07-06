using System;

namespace com.tibbo.aggregate.common.@event
{
    public class EventHandlingException : AggreGateException
    {
        public EventHandlingException(String message) : base(message)
        {
        }

        public EventHandlingException(Exception cause) : base(cause)
        {
        }

        public EventHandlingException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}