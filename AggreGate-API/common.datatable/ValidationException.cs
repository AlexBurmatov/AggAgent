using System;

namespace com.tibbo.aggregate.common.datatable
{
    public class ValidationException : Exception
    {
        public ValidationException(String message) : base(message)
        {
        }

        public ValidationException(Exception cause) : base("", cause)
        {
        }

        public ValidationException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}