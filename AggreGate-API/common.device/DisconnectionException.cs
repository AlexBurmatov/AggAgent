using System;

namespace com.tibbo.aggregate.common.device
{
    public class DisconnectionException : AggreGateException
    {
        public DisconnectionException(String message) : base(message)
        {
        }

        public DisconnectionException(Exception cause) : base(cause)
        {
        }

        public DisconnectionException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}