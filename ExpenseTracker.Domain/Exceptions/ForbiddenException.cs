using System;

namespace ExpenseTracker.Domain.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string exception) : base(exception) { }
    }
}
