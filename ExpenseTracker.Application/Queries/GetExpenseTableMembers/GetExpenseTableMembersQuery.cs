using ExpenseTracker.Application.DTO;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetExpenseTableMembers
{
    public record GetExpenseTableMembersQuery(int ExpenseTableId) : IRequest<IReadOnlyList<ExpenseTableMemberDto>>;
}
