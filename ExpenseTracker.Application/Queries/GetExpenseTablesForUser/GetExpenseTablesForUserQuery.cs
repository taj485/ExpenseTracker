using ExpenseTracker.Application.DTO;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetExpenseTablesForUser
{
    public class GetExpenseTablesForUserQuery : IRequest<IReadOnlyList<ExpenseTableDto>>
    {
    }
}
