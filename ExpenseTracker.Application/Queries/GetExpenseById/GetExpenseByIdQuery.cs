using ExpenseTracker.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Queries.GetExpenseById
{
    public class GetExpenseByIdQuery : IRequest<ExpenseDto>
    {
        public int Id { get; set; }
    }
}
