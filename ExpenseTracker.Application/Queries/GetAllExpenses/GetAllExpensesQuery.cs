using ExpenseTracker.Application.DTO;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetAllExpenses
{
    public class GetAllExpensesQuery : IRequest<IReadOnlyList<ExpenseDto>>
    {
        public int ExpenseTableId { get; set; }
    }
}
