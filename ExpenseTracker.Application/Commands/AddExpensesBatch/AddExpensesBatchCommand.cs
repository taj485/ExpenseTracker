using MediatR;
using System.Collections.Generic;
using ExpenseTracker.Application.Commands.AddExpense;

namespace ExpenseTracker.Application.Commands.AddExpensesBatch
{
    /// <param name="TempImageReference">
    /// Name of the receipt image still sitting in the temp container. Promoted to permanent storage
    /// when the batch actually saves; left to the container's lifecycle rule if the user abandons.
    /// </param>
    public record AddExpensesBatchCommand(int ExpenseTableId, List<AddExpenseCommand> Items, string? TempImageReference = null) : IRequest<AddExpensesBatchResult>;

    public record BatchItemError(int Index, List<string> Errors);

    public record AddExpensesBatchResult(List<int> AddedIds, List<BatchItemError> Errors);
}
