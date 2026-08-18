using ExpenseTracker.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpenseTracker.Application.Commands.AddExpense
{
    public record AddExpenseCommand(int ExpenseTableId, decimal UnitPrice, ExpenseCategory Category, string Description, DateTime Date, string? Merchant = null, int Quantity = 1) : IRequest<int>;
}
