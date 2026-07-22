using MediatR;

namespace ExpenseTracker.Application.Commands.UnstarExpenseTable
{
    public record UnstarExpenseTableCommand(int ExpenseTableId) : IRequest<Unit>;
}
