using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string exception) : base(exception) { }
    }
}


    