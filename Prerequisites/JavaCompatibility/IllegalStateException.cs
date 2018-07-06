using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaCompatibility
{
    public class IllegalStateException : InvalidOperationException
    {
        public IllegalStateException(string messageString, Exception causeException) : base(messageString, causeException)
        {
        }
    }
}
