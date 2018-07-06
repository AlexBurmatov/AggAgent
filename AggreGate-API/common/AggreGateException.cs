using System;

namespace com.tibbo.aggregate.common
{
    public class AggreGateException : Exception
    {
        private readonly String details;

        public AggreGateException(String message) : base(message)
        {
        }

        public AggreGateException(String message, String details) : base(message)
        {
            this.details = details;
        }

        public AggreGateException(Exception cause) : base(cause.Message, cause)
        {
        }

        public AggreGateException(String message, Exception cause) : base(message, cause)
        {
        }

        public AggreGateException(String message, Exception cause, String details) : base(message, cause)
        {
            this.details = details;
        }

        public String getDetails()
        {
            return details;
        }
    }
}