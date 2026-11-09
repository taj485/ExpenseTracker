using System;

namespace ExpenseTracker.Domain.Exceptions
{
    /// <summary>
    /// An external service the request depends on failed. Mapped to 502 with the message as the error.
    /// </summary>
    public class ExternalServiceException : Exception
    {
        public ExternalServiceException(string message, Exception? innerException = null) : base(message, innerException) { }
    }
}
