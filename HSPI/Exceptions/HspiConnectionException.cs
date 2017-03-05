using System;

namespace Hspi.Exceptions
{
    public class HspiConnectionException : HspiException
    {
        public HspiConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}