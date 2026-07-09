using ExpenseTracker.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Commands.AddExpense
{
    public record AddExpenseCommand(decimal Amount, ExpenseCategory Category, string Description, DateTime Date) : IRequest<int>;
}
