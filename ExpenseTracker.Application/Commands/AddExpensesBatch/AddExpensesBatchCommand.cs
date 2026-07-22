using MediatR;
using System.Collections.Generic;
using ExpenseTracker.Application.Commands.AddExpense;

namespace ExpenseTracker.Application.Commands.AddExpensesBatch
{
    public record AddExpensesBatchCommand(int ExpenseTableId, List<AddExpenseCommand> Items) : IRequest<AddExpensesBatchResult>;

    public record BatchItemError(int Index, List<string> Errors);

    public record AddExpensesBatchResult(List<int> AddedIds, List<BatchItemError> Errors);
}
