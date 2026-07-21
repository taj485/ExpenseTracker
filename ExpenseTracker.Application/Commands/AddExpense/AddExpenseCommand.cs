using ExpenseTracker.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Commands.AddExpense
{
    public record AddExpenseCommand(int ExpenseTableId, decimal Amount, ExpenseCategory Category, string Description, DateTime Date, string? Merchant = null) : IRequest<int>;
}
