using ExpenseTracker.Application.DTO;
using MediatR;

namespace ExpenseTracker.Application.Queries.GetExpensesByReceiptId
{
    public class GetExpensesByReceiptIdQuery : IRequest<IReadOnlyList<ExpenseDto>>
    {
        public int ReceiptId { get; set; }
        public int ExpenseTableId { get; set; }
    }
}
