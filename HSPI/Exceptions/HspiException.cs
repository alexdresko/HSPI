using System;
using System.Runtime.Serialization;

namespace Hspi.Exceptions
{
    public class HspiException : ApplicationException
    {
        public HspiException()
        {
        }

        public HspiException(string message) : base(message)
        {
        }

        public HspiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HspiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}