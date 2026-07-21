using MediatR;

namespace ExpenseTracker.Application.Commands.DeleteExpenseTable
{
    public record DeleteExpenseTableCommand(int ExpenseTableId) : IRequest<Unit>;
}
