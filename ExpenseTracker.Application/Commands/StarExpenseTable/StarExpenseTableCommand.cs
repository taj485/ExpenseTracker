using MediatR;

namespace ExpenseTracker.Application.Commands.StarExpenseTable
{
    public record StarExpenseTableCommand(int ExpenseTableId) : IRequest<Unit>;
}
