using System;

namespace com.tibbo.aggregate.common.device
{
    public class RemoteDeviceErrorException : AggreGateException
    {
        public RemoteDeviceErrorException(string message) : base(message)
        {
        }

        public RemoteDeviceErrorException(string message, string details) : base(message, details)
        {
        }

        public RemoteDeviceErrorException(Exception cause) : base(cause)
        {
        }

        public RemoteDeviceErrorException(string message, Exception cause) : base(message, cause)
        {
        }

        public RemoteDeviceErrorException(string message, Exception cause, string details)
            : base(message, cause, details)
        {
        }
    }
}